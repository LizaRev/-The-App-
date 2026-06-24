using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MessengerGui.Models
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }
        
        public string Title { get; set; } = "";
        
        [NotMapped] 
        public List<int> ParticipantIds { get; set; } = new List<int>();
        
        // Для зберігання списку ID у SQLite як одного рядка (напр: "1,2,3")
        public string ParticipantIdsString 
        { 
            get => string.Join(",", ParticipantIds);
            set => ParticipantIds = string.IsNullOrWhiteSpace(value) 
                ? new List<int>() 
                : value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        }
    }
}