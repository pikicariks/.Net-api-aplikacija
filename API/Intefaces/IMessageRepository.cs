using System;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Intefaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);

    Task<Message?> GetMessage(int id);
    Task<PageList<MessageDTO>> GetMessagesForUser(MessageParams messageParams);

    Task<IEnumerable<MessageDTO>> GetMessageThread(string username,string recipient);

   

    void AddGroup(Group group);

    void RemoveConnection(Connection connection);

    Task<Connection?> GetConnection(string connectionId);

    Task<Group?> GetGroup(string groupName);

    Task<Group?> GetGroupForConnection(string id);
}
