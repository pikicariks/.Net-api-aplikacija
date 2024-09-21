using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Intefaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Authorize]
public class MessagesController(IUnitOfWork unitOfWork , IMapper mapper):BaseApiController
{

    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO){
        var username = User.GetUsername();
        if (username==createMessageDTO.RecipientUsername.ToLower())
        {
            return BadRequest("You cannot message yourself");
        }

        var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

        if (recipient==null || sender==null || sender.UserName==null || recipient.UserName==null)
        {
            return BadRequest("Cannot send message at this time");
        }

        var message = new Message{
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsernam = recipient.UserName,
            Content = createMessageDTO.Content
        };

        unitOfWork.MessageRepository.AddMessage(message);
        if(await unitOfWork.Complete()) return Ok(mapper.Map<MessageDTO>(message));

        return BadRequest("Failed to save message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams){

        messageParams.Username = User.GetUsername();
        var messages = await unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages);

        return messages;

    }

     [HttpGet("thread/{username}")]
     public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username){

        var current = User.GetUsername();
        return Ok(await unitOfWork.MessageRepository.GetMessageThread(current,username));
     }

     
     [HttpDelete("{id}")]
     public async Task<ActionResult> DeleteMessage(int id){

        var current = User.GetUsername();
        var message = await unitOfWork.MessageRepository.GetMessage(id);

        if(message==null) return BadRequest("Cannot delete this message");

        if(message.SenderUsername!=current && message.RecipientUsernam!=current)return Forbid();

        if(message.SenderUsername==current) message.SenderDeleted=true;
        if(message.RecipientUsernam==current) message.RecipientDeleted=true;
        if(message is {SenderDeleted:true,RecipientDeleted:true}){
            unitOfWork.MessageRepository.DeleteMessage(message);
        }

        if(await unitOfWork.Complete())  return Ok();

        return BadRequest("Problem deleting the message");
       
     }
}
