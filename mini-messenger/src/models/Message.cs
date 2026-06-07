using System;
using System.ComponentModel.DataAnnotations;

public class Message
{
    [Key]
    public int Id { get; set; }
    
    public int ConversationId { get; set; }
    
    public int SenderId { get; set; }
    
    public string Text { get; set; } = "";
    
    public DateTime Timestamp { get; set; } 
}