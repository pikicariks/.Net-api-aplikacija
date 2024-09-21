using System;
using API.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class PresenceHub(PresenceTracker presenceTracker):Hub
{
    public  override async Task OnConnectedAsync()
    {

        if (Context.User==null)
        {
            throw new HubException("Cannot get user claim");
        }
        var isonline = await presenceTracker.UserConnected(Context.User.GetUsername(),Context.ConnectionId);
       
         if (isonline) await Clients.Others.SendAsync("UserIsOnline",Context.User?.GetUsername());
        var currentUsers = await presenceTracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers",currentUsers);
    
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
         if (Context.User==null)
        {
            throw new HubException("Cannot get user claim");
        }
        var isoffline = await presenceTracker.UserDisConnected(Context.User.GetUsername(),Context.ConnectionId);

        if(isoffline) await Clients.Others.SendAsync("UserIsOffline",Context.User?.GetUsername()); 
        

       

        await base.OnDisconnectedAsync(exception);

    }
}
