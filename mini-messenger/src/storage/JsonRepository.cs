using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class DataStorage
{
    public List<User> Users { get; set; } = new List<User>();
    public List<Conversation> Conversations { get; set; } = new List<Conversation>();
    public List<Message> Messages { get; set; } = new List<Message>();
}

public class JsonRepository : IRepository
{
    private readonly string filePath = "data/messages.json";
    private DataStorage _data = new DataStorage();

    public List<User> Users => _data.Users;
    public List<Conversation> Conversations => _data.Conversations;
    public List<Message> Messages => _data.Messages;

    public JsonRepository()
    {
        if (!Directory.Exists("data")) Directory.CreateDirectory("data");
        Load();
    }

    public List<User> GetUsers() => _data.Users;
    public void AddUser(User user) { _data.Users.Add(user); Save(); }

    public List<Message> GetMessagesForConversation(int convId) 
        => _data.Messages.Where(m => m.ConversationId == convId).ToList();

    public bool AddMessage(Message msg)
    {
        if (string.IsNullOrWhiteSpace(msg.Text)) return false;
        _data.Messages.Add(msg);
        Save();
        return true;
    }

    public List<Conversation> GetConversationsForUser(int userId)
        => _data.Conversations.Where(c => c.ParticipantIds.Contains(userId)).ToList();

    public bool AddConversation(Conversation conv)
    {
        _data.Conversations.Add(conv);
        Save();
        return true;
    }

    private void Save()
    {
        string json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private void Load()
    {
        if (!File.Exists(filePath)) return;
        string json = File.ReadAllText(filePath);
        try { _data = JsonSerializer.Deserialize<DataStorage>(json) ?? new DataStorage(); }
        catch { }
    }
}