using System;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Intefaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace API.Data;

public class MessageRepository(DataContext dataContext,IMapper mapper) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        dataContext.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        dataContext.Messages.Remove(message);
    }

    public async Task<Message?> GetMessage(int id)
    {
        return await dataContext.Messages.FindAsync(id);
    }

    public async Task<PageList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = dataContext.Messages
        .OrderByDescending(x=>x.MessageSent)
        .AsQueryable();

        query = messageParams.Container switch{
            "Inbox"=>query.Where(x=>x.Recipient.UserName==messageParams.Username && x.RecipientDeleted==false),
            "Outbox"=>query.Where(x=>x.Sender.UserName==messageParams.Username && x.SenderDeleted==false),
            _ => query.Where(x=>x.Recipient.UserName==messageParams.Username && x.DateRead==null
            && x.RecipientDeleted==false)
        };

        var messages = query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);
    
        return await PageList<MessageDTO>.CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string username, string recipient)
    {
        var messages = await dataContext.Messages
        .Include(x=>x.Sender).ThenInclude(x=>x.Photos)
        .Include(x=>x.Recipient).ThenInclude(x=>x.Photos)
        .Where(x=>x.RecipientUsernam==username && x.RecipientDeleted==false && x.SenderUsername==recipient
        || x.SenderUsername==username && x.SenderDeleted==false && x.RecipientUsernam==recipient)
        .OrderBy(x=>x.MessageSent)
        .ToListAsync();

        var unread = messages.Where(x=>x.DateRead==null && x.RecipientUsernam==username)
        .ToList();

        if(unread.Count !=0){
            unread.ForEach(x=>x.DateRead=DateTime.UtcNow);
            await dataContext.SaveChangesAsync();
        }

        return mapper.Map<IEnumerable<MessageDTO>>(messages);

    }

    public async Task<bool> SaveAllAsync()
    {
        return await dataContext.SaveChangesAsync()>0;
    }
}
