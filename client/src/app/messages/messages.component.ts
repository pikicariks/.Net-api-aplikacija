import { Component, inject, OnInit } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FormsModule, NgModel } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from '../_models/message';
import { RouterLink } from '@angular/router';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [ButtonsModule,FormsModule,TimeagoModule,RouterLink,PaginationModule],
  templateUrl: './messages.component.html',
  styleUrl: './messages.component.css'
})
export class MessagesComponent implements OnInit {
ngOnInit(): void {
 this.load();
}
messageService = inject(MessageService);

container = 'Inbox';
pageNum = 1;
pageS = 5;
isOutbox = this.container == 'Outbox';

load(){
  this.messageService.getMessages(this.pageNum,this.pageS,this.container);
}

delete(id:number){
  this.messageService.deleteMessage(id).subscribe({
    next:()=>{
      this.messageService.paginatedResult.update(prev => {
        if(prev && prev.items){
          prev.items.splice(prev.items.findIndex(m=>m.id===id),1);
          return prev;
        }
        return prev;
      })
    }
  })
}

getRoute(message:Message){
  if(this.container=='Outbox') return `/members/${message.recipientUsernam}`;
  else{
    return `/members/${message.senderUsername}`;
  }
}

pageChanged(event:any){
  if (this.pageNum!==event.page) {
    this.pageNum = event.page;
    this.load();
  }
}
}
