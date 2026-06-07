public interface IRepository
{
    List<User> Users { get; }
    List<Conversation> Conversations { get; }
    List<Message> Messages { get; }
    List<User> GetUsers();
    void AddUser(User user);
    List<Message> GetMessagesForConversation(int conversationId);
    bool AddMessage(Message message);
    List<Conversation> GetConversationsForUser(int userId);
    bool AddConversation(Conversation conversation);
}