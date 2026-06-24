using System;
using System.Collections.Generic;
using System.Linq;

public class SqliteRepository : IRepository
{
    private readonly SqliteContext _db = new SqliteContext();

    public SqliteRepository() => _db.Database.EnsureCreated();

    public List<User> Users => _db.Users.ToList();
    public List<Conversation> Conversations => _db.Conversations.ToList();
    public List<Message> Messages => _db.Messages.ToList();

    // Методи, які вимагає IRepository
    public List<User> GetUsers() => _db.Users.ToList();

    public void AddUser(User user) 
    {
        _db.Users.Add(user);
        _db.SaveChanges();
    }

    public List<Message> GetMessagesForConversation(int conversationId)
    {
        return _db.Messages
                  .Where(m => m.ConversationId == conversationId)
                  .OrderBy(m => m.Timestamp)
                  .ToList();
    }

    public bool AddMessage(Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Text)) return false;
        
        _db.Messages.Add(message);
        _db.SaveChanges();
        return true;
    }

    public List<Conversation> GetConversationsForUser(int userId)
    {
        return _db.Conversations.AsEnumerable()
                  .Where(c => c.ParticipantIds.Contains(userId))
                  .ToList();
    }

    public bool AddConversation(Conversation conversation)
    {
        _db.Conversations.Add(conversation);
        _db.SaveChanges();
        return true;
    }
}