import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { take } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubsUrl;
  private hubConnection? :HubConnection;
private toaster = inject(ToastrService);
online = signal<string[]>([]);
private router = inject(Router);
  

createHubConnection(user:User){
  this.hubConnection = new HubConnectionBuilder()
  .withUrl(this.hubUrl+'presence',{
    accessTokenFactory:()=>user.token
    
  })
  .withAutomaticReconnect()
  .build();

  this.hubConnection.start().catch(error=>console.log(error));

  this.hubConnection.on("UserIsOnline",username=>{
    this.online.update(users=>[...users,username]);
  });

  this.hubConnection.on("UserIsOffline",username=>{
    this.online.update(users=>users.filter(x=>x!=username));
  });

  this.hubConnection.on('GetOnlineUsers',usernames=>{
    this.online.set(usernames)
  });

  this.hubConnection.on('NewMessageReceived',({username,knownAs})=>{
    this.toaster.info(knownAs + 'has sent you a new message!Click me to see it')
    .onTap.pipe(take(1))
    .subscribe(()=>this.router.navigateByUrl('/members/'+username+'?tab=messages'))
  });
}

stopHubConn(){
  if (this.hubConnection?.state === HubConnectionState.Connected) {
    this.hubConnection.stop().catch(error=>console.log(error));
  }
}
}
