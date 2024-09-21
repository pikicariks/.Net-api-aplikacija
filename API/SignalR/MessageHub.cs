using System;
using System.Security.AccessControl;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Intefaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IUnitOfWork unitOfWork,
IMapper mapper,IHubContext<PresenceHub> presenceHub):Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpCon = Context.GetHttpContext();
        var otherUser = httpCon?.Request.Query["user"];

        if(Context.User==null || string.IsNullOrEmpty(otherUser) ) throw new Exception("Cannot join group");
        var groupName = GetGroupName(Context.User.GetUsername(),otherUser);

        await Groups.AddToGroupAsync(Context.ConnectionId,groupName);
        var group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup",group);


        var messages = await unitOfWork.MessageRepository.GetMessageThread(Context.User.GetUsername(),otherUser!);
        
        if (unitOfWork.HasChanges())
        {
            await unitOfWork.Complete();
        }
        
        await Clients.Caller.SendAsync("ReceiveMessageThread",messages);
    }

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup",group);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDTO createMessageDTO){

             var username = Context.User?.GetUsername() ?? throw new Exception("could not get user");
        if (username==createMessageDTO.RecipientUsername.ToLower())
        {
            throw new HubException("cannot message yourself");
        }

        var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

        if (recipient==null || sender==null || sender.UserName==null || recipient.UserName==null)
        {
            throw new HubException("cannot message yourself");
        }

        var message = new Message{
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsernam = recipient.UserName,
            Content = createMessageDTO.Content
        };

        var groupName = GetGroupName(sender.UserName,recipient.UserName);
        var group = await unitOfWork.MessageRepository.GetGroup(groupName);

        if (group!=null && group.Connections.Any(x=>x.Username==recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else{
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if (connections!=null && connections?.Count!=null)
            {
                await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new {
                    username = sender.UserName,knownAs = sender.KnownAs
                });
            }
        }

        unitOfWork.MessageRepository.AddMessage(message);
        if(await unitOfWork.Complete()) {
            
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDTO>(message));
        }

        
    }

    private async Task<Group> AddToGroup(string name){
        var usename = Context.User?.GetUsername() ?? throw new Exception("Cannot get username");

        var group = await unitOfWork.MessageRepository.GetGroup(name);

        var connection = new Connection{ConnectionId = Context.ConnectionId,Username=usename};

        if(group==null){
            group = new Group{Name=name};
            unitOfWork.MessageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        if(await unitOfWork.Complete()) return group;

        throw new HubException("Failed to join group");


    }

    private async Task<Group> RemoveFromMessageGroup(){
        var group = await unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
        var conn = group?.Connections.FirstOrDefault(x=>x.ConnectionId==Context.ConnectionId);

        if (conn!=null && group!=null)
        {
            unitOfWork.MessageRepository.RemoveConnection(conn);
           if(await unitOfWork.Complete()) return group;
        }

        throw new Exception("Failed to remove from group");
    }
    private string GetGroupName(string? caller, string? other){
        var stringCompare = string.CompareOrdinal(caller,other) < 0;

        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
