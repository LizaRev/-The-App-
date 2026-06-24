using System;

namespace MessengerGui.Models;

public class MessageViewModel
{
    public int Id { get; set; } 
    public string SenderName { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public bool IsCurrentUser { get; set; }
    public bool ShowDateHeader { get; set; }
    public string DateHeader => Timestamp.ToString("d MMMM yyyy");
}