using System;

namespace API.SignalR;

public class PresenceTracker
{
    private static readonly Dictionary<string,List<string>> OnlineUsers = [];

    public Task<bool> UserConnected(string username,string connId){
        var isonline = false;
        
        lock (OnlineUsers){
            if(OnlineUsers.ContainsKey(username)){
                OnlineUsers[username].Add(connId);
            }

            else{
                OnlineUsers.Add(username,[connId]);
                isonline=true;
            }
        }

        return Task.FromResult(isonline);

    }

     public Task<bool> UserDisConnected(string username,string connId){
        var isoffline = false;
        lock (OnlineUsers){
            if(!OnlineUsers.ContainsKey(username)){
               return Task.FromResult(isoffline);
            }

            
                OnlineUsers[username].Remove(connId);
             if(OnlineUsers[username].Count==0)  {
                OnlineUsers.Remove(username);
                isoffline=true;
             } 
            
        }

        return Task.FromResult(isoffline);

    }

    public Task<string[]> GetOnlineUsers(){
        string[] onlineUsers;

        lock (OnlineUsers) {
            onlineUsers = OnlineUsers.OrderBy(x=>x.Key).Select(x=>x.Key).ToArray();

        }

        return Task.FromResult(onlineUsers);
    }

    public static Task<List<string>> GetConnectionsForUser(string username){
        List<string> connectionIds;

        if (OnlineUsers.TryGetValue(username,out var connections))
        {
            lock (connections){
                connectionIds = [.. connections];
            }
        }
        else{
            connectionIds = [];
        }

        return Task.FromResult(connectionIds);
    }

}
