using System;
using System.Collections.Generic;

public class JsonRepository
{
    
    public List<User> Users = new List<User>();
    public List<Conversation> Conversations = new List<Conversation>();
    public List<Message> Messages = new List<Message>();

    public void AddUser(User user)
    {
        Users.Add(user);
    }

    public void AddMessage(Message message)
    {
        Messages.Add(message);
    }

    public List<Message> GetMessagesForConversation(int conversationId)
    {
        List<Message> filteredMessages = new List<Message>();
        
        foreach (var msg in Messages)
        {
            if (msg.ConversationId == conversationId)
            {
                filteredMessages.Add(msg);
            }
        }
        
        return filteredMessages;
    }
}