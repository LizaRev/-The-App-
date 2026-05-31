public class Conversation
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<int> ParticipantIds { get; set; } = new List<int>(); 
}

