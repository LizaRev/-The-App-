using System;
using System.Collections.Generic;
using System.Linq;
using MessengerGui.Models;

namespace MessengerGui.Storage
{
    public class SqliteRepository : IRepository
    {
        private readonly SqliteContext _db = new SqliteContext();

        public SqliteRepository() => _db.Database.EnsureCreated();

        public List<User> Users => _db.Users.ToList();
        public List<Conversation> Conversations => _db.Conversations.ToList();
        public List<Message> Messages => _db.Messages.ToList();

        public List<User> GetUsers() => _db.Users.ToList();

        public void AddUser(User user) 
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public void DeleteUser(int userId)
        {
            var user = _db.Users.Find(userId);
            if (user != null)
            {
                var userChats = _db.Conversations.AsEnumerable()
                                   .Where(c => c.ParticipantIds.Contains(userId))
                                   .ToList();
                foreach (var chat in userChats) { DeleteConversation(chat.Id); }
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
        }

        public List<Message> GetMessagesForConversation(int conversationId)
        {
            return _db.Messages.Where(m => m.ConversationId == conversationId).OrderBy(m => m.Timestamp).ToList();
        }

        public bool AddMessage(Message message)
        {
            if (string.IsNullOrWhiteSpace(message.Text)) return false;
            _db.Messages.Add(message);
            _db.SaveChanges();
            return true;
        }

        public void DeleteMessage(int messageId)
        {
            var message = _db.Messages.Find(messageId);
            if (message != null) { _db.Messages.Remove(message); _db.SaveChanges(); }
        }

        public List<Conversation> GetConversationsForUser(int userId)
        {
            return _db.Conversations.AsEnumerable().Where(c => c.ParticipantIds.Contains(userId)).ToList();
        }

        public bool AddConversation(Conversation conversation)
        {
            _db.Conversations.Add(conversation);
            _db.SaveChanges();
            return true;
        }

        public void DeleteConversation(int conversationId)
        {
            var conversation = _db.Conversations.Find(conversationId);
            if (conversation != null)
            {
                _db.Messages.RemoveRange(_db.Messages.Where(m => m.ConversationId == conversationId));
                _db.Conversations.Remove(conversation);
                _db.SaveChanges();
            }
        }
    }
}