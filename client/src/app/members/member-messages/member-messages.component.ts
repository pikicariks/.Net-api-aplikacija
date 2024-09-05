import { Component, inject, input, OnInit, output, ViewChild } from '@angular/core';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
import { TimeagoModule } from 'ngx-timeago';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  imports: [TimeagoModule,FormsModule],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css'
})
export class MemberMessagesComponent {
@ViewChild('messageForm') messageForm?:NgForm;

  username = input.required<string>();
  messages = input.required<Message[]>();
  content = '';
  upmessages = output<Message>();

  private messageService = inject(MessageService);

  sendMessage(){
    this.messageService.sendMessage(this.username(),this.content).subscribe({
      next:message=>{
        this.upmessages.emit(message);
        this.messageForm?.reset();

      }
    })
  }

}
