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

    // --- ВАЛІДАЦІЯ (День 5) ---
    public string ValidateMessage(Message msg)
    {
        if (string.IsNullOrWhiteSpace(msg.Text)) return "Помилка: Повідомлення не може бути порожнім.";
        if (msg.Text.Length > 4000) return "Помилка: Повідомлення занадто довге (ліміт 4000 символів).";
        return "OK";
    }

    public string ValidateConversation(Conversation conv)
    {
        if (conv.ParticipantIds == null || conv.ParticipantIds.Count == 0) 
            return "Помилка: Чат повинен мати хоча б одного учасника.";
        return "OK";
    }
    // -------------------------

    public bool AddMessage(Message message)
    {
        string error = ValidateMessage(message);
        if (error != "OK") { Console.WriteLine(error); return false; }
        
        Messages.Add(message);
        Save();
        return true;
    }

    public bool AddConversation(Conversation conv)
    {
        string error = ValidateConversation(conv);
        if (error != "OK") { Console.WriteLine(error); return false; }

        Conversations.Add(conv);
        Save();
        return true;
    }

    public void AddUser(User user) { Users.Add(user); Save(); }

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
        catch { }
    }

    public List<Message> GetMessagesForConversation(int conversationId) => Messages.Where(m => m.ConversationId == conversationId).ToList();
}