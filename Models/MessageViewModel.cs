using System;

namespace MessengerGui.Models;

public class MessageViewModel {
    public int Id { get; set; }
    
    public string Text { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public bool IsCurrentUser { get; set; }
    
    public string TimeDisplay => Timestamp.ToString("HH:mm");
    public string DateHeader => Timestamp.ToString("dd.MM.yyyy");
    
    public bool ShowDateHeader { get; set; } 
}