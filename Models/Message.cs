using System;

namespace MessengerGui.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string Text { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public string? FilePath { get; set; } // Для фото/відео
    }
}