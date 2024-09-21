import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PaginationResult } from '../_models/pagination';
import { Message } from '../_models/message';
import { setPaginatedResponse, setPaginationHeaders } from './paginationHelper';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { User } from '../_models/user';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root'
})
export class MessageService {


baseUrl = environment.apiUrl;
private http=inject(HttpClient);

paginatedResult = signal<PaginationResult<Message[]> | null>(null);
huburl = environment.hubsUrl;
 hubConnection?:HubConnection;
thread = signal<Message[]>([]);

createHubConnection(user:User,other:string){
  this.hubConnection = new HubConnectionBuilder()
  .withUrl(this.huburl+'message?user='+other,{
    accessTokenFactory:()=>user.token
  })
  .withAutomaticReconnect().build();

  this.hubConnection.start().catch(error=>console.log(error));

  this.hubConnection.on('ReceiveMessageThread',messages=>{
    this.thread.set(messages);
  });

  
  this.hubConnection.on("NewMessage",message=>{
    this.thread.update(messages=>[...messages,message])
  });

  this.hubConnection.on("UpdatedGroup",(group:Group)=>{
    if(group.connections.some(x=>x.username==other)){
      this.thread.update(messages=>{
        messages.forEach(message=>{
          if (!message.dateRead) {
            message.dateRead = new Date(Date.now());
          }
        })
      return messages;
      })
    }
  });

}



stopHubConnection()
{
  if(this.hubConnection?.state==HubConnectionState.Connected){
    this.hubConnection.stop().catch(error=>console.log(error));
  }
}

getMessages(pageNum:number,pageSize:number,container:string){

  let params = setPaginationHeaders(pageNum,pageSize);

  params = params.append('Container',container);

  return this.http.get<Message[]>(this.baseUrl+'messages',{observe:'response',params})
  .subscribe({
    next:response=>setPaginatedResponse(response,this.paginatedResult)
  })
}


getMessageThread(username:string){
  return this.http.get<Message[]>(this.baseUrl+'messages/thread/'+username);
}

async sendMessage(username:string,content:string){
  return this.hubConnection?.invoke('SendMessage',{recipientUsername:username,content})
}

deleteMessage(id:number){
  return this.http.delete(this.baseUrl+'messages/'+id);
}
} 
