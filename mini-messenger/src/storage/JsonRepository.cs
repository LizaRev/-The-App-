using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

// Окремий клас-контейнер для даних
public class DataStorage
{
    public List<User> Users { get; set; } = new List<User>();
    public List<Conversation> Conversations { get; set; } = new List<Conversation>();
    public List<Message> Messages { get; set; } = new List<Message>();
}

public class JsonRepository
{
    private readonly string filePath = "data/messages.json";
    public List<User> Users { get; set; } = new List<User>();
    public List<Conversation> Conversations { get; set; } = new List<Conversation>();
    public List<Message> Messages { get; set; } = new List<Message>();

    public JsonRepository()
    {
        if (!Directory.Exists("data")) Directory.CreateDirectory("data");
        Load();
    }

    public void Save()
    {
        var data = new DataStorage { Users = this.Users, Conversations = this.Conversations, Messages = this.Messages };
        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public void Load()
    {
        if (!File.Exists(filePath)) return;
        string json = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(json)) return;

        try
        {
            var data = JsonSerializer.Deserialize<DataStorage>(json);
            if (data != null)
            {
                this.Users = data.Users ?? new List<User>();
                this.Conversations = data.Conversations ?? new List<Conversation>();
                this.Messages = data.Messages ?? new List<Message>();
            }
        }
        catch { /* Ігноруємо помилки */ }
    }

    public void AddUser(User user) { Users.Add(user); Save(); }
    public void AddMessage(Message message) { Messages.Add(message); Save(); }
    public void AddConversation(Conversation conversation) { Conversations.Add(conversation); Save(); }
    public List<Message> GetMessagesForConversation(int conversationId) => Messages.Where(m => m.ConversationId == conversationId).ToList();
}