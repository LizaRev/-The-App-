using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

public class Conversation
{
    [Key]
    public int Id { get; set; }
    
    public string Title { get; set; } = "";
    
    // Ігноруємо це поле в базі, бо список не можна зберегти прямо в колонку SQLite
    [NotMapped] 
    public List<int> ParticipantIds { get; set; } = new List<int>();
    
    // Це поле буде зберігатися в БД як текст: "1,2,3"
    public string ParticipantIdsString 
    { 
        get => string.Join(",", ParticipantIds);
        set => ParticipantIds = value.Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                                     .Select(int.Parse).ToList();
    }
}