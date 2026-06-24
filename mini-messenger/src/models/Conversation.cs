using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

public class Conversation
{
    [Key]
    public int Id { get; set; }
    
    public string Title { get; set; } = "";
    
    [NotMapped] 
    public List<int> ParticipantIds { get; set; } = new List<int>();
    
    public string ParticipantIdsString 
    { 
        get => string.Join(",", ParticipantIds);
        set => ParticipantIds = value.Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                                     .Select(int.Parse).ToList();
    }
}