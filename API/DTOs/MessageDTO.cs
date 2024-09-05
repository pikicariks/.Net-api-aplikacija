using System;

namespace API.DTOs;

public class MessageDTO
{
public int Id { get; set; }
public int SenderId { get; set; }

    public required string SenderUsername { get; set; }

    public required string SenderPhotoUrl { get; set; }

    public int RecipientId { get; set; }

    public required string RecipientUsernam { get; set; }

    public required string RecipientPhotoUrl { get; set; }

    public required string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; } 
    

    
    
}
