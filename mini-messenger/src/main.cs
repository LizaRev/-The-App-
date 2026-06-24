using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        IRepository repository = new SqliteRepository(); 
        bool isRunning = true;

        while (isRunning)
        {
            User? currentUser = null;

            while (currentUser == null && isRunning)
            {
                Console.Clear();
                Console.WriteLine("=== ВХІД В МЕСЕНДЖЕР (SQLite) ===");
                foreach (var u in repository.GetUsers())
                    Console.WriteLine(u.Id + ". Увійти як " + u.Username);
                
                Console.WriteLine("-------------------------");
                Console.WriteLine("R - Зареєструватись | /quit - Вихід");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine() ?? "";
                if (choice.ToLower() == "/quit") { isRunning = false; break; }
                
                if (choice.ToLower() == "r")
                {
                    Console.Write("Введіть ім'я: ");
                    string name = Console.ReadLine() ?? "";
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        var users = repository.GetUsers();
                        int newId = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
                        repository.AddUser(new User { Id = newId, Username = name });
                        Console.WriteLine("Користувача створено!");
                        Console.ReadKey();
                    }
                }
                else if (int.TryParse(choice, out int userId))
                    currentUser = repository.GetUsers().FirstOrDefault(u => u.Id == userId);
            }

            if (!isRunning) break;

            Conversation? activeConversation = null;
            while (activeConversation == null && isRunning)
            {
                Console.Clear();
                Console.WriteLine("Користувач: " + currentUser!.Username);
                Console.WriteLine("Оберіть чат:");

                var userChats = repository.GetConversationsForUser(currentUser.Id);
                for (int i = 0; i < userChats.Count; i++)
                {
                    var chat = userChats[i];
                    string displayName = chat.Title;
                    // Знаходимо іншого учасника чату для назви
                    var otherId = chat.ParticipantIds.FirstOrDefault(id => id != currentUser.Id);
                    if (otherId != 0)
                    {
                        var otherUser = repository.GetUsers().FirstOrDefault(u => u.Id == otherId);
                        if (otherUser != null) displayName = "Чат з " + otherUser.Username;
                    }
                    Console.WriteLine((i + 1) + ". " + displayName);
                }
                Console.WriteLine((userChats.Count + 1) + ". [Створити новий чат]");
                Console.WriteLine("0. Вийти з акаунту");
                Console.Write("> ");

                string chatChoice = Console.ReadLine() ?? "";
                if (chatChoice == "0") break;

                if (int.TryParse(chatChoice, out int index))
                {
                    if (index <= userChats.Count) activeConversation = userChats[index - 1];
                    else if (index == userChats.Count + 1)
                    {
                        Console.WriteLine("Оберіть ID користувача:");
                        foreach (var u in repository.GetUsers().Where(u => u.Id != currentUser.Id))
                            Console.WriteLine(u.Id + ". " + u.Username);
                        
                        if (int.TryParse(Console.ReadLine(), out int otherId))
                        {
                            var otherUser = repository.GetUsers().FirstOrDefault(u => u.Id == otherId);
                            if (otherUser != null)
                            {
                                var newConv = new Conversation { Title = "Чат" };
                                newConv.ParticipantIds.Add(currentUser.Id);
                                newConv.ParticipantIds.Add(otherUser.Id);
                                if (repository.AddConversation(newConv)) activeConversation = newConv;
                            }
                        }
                    }
                }
            }

            if (activeConversation == null) continue;

            while (true)
            {
                string chatTitle = activeConversation.Title;
                var otherId = activeConversation.ParticipantIds.FirstOrDefault(id => id != currentUser!.Id);
                if (otherId != 0)
                {
                    var otherUser = repository.GetUsers().FirstOrDefault(u => u.Id == otherId);
                    if (otherUser != null) chatTitle = "Чат з " + otherUser.Username;
                }

                Console.Clear();
                Console.WriteLine("=== " + chatTitle + " ===");
                Console.WriteLine("ДОПОМОГА: /search [текст] | /logout - вийти | /quit - закрити");
                Console.WriteLine("-----------------------------------");

                foreach (var msg in repository.GetMessagesForConversation(activeConversation.Id))
                {
                    var sender = repository.GetUsers().FirstOrDefault(u => u.Id == msg.SenderId);
                    Console.WriteLine($"[{msg.Timestamp:HH:mm:ss}] {sender?.Username ?? "Unknown"}: {msg.Text}");
                }

                Console.WriteLine("-----------------------------------");
                Console.Write("> ");

                string input = Console.ReadLine() ?? "";
                if (input.ToLower() == "/quit") { isRunning = false; break; }
                if (input.ToLower() == "/logout") break;

                if (input.ToLower().StartsWith("/search"))
                {
                    if (input.Length > 8)
                    {
                        string query = input.Substring(8);
                        var results = repository.GetMessagesForConversation(activeConversation.Id)
                                                .Where(m => m.Text.Contains(query, StringComparison.OrdinalIgnoreCase));
                        Console.WriteLine("\n--- Результати пошуку '" + query + "' ---");
                        foreach (var m in results)
                            Console.WriteLine("[" + m.Timestamp.ToString("HH:mm") + "] " + m.Text);
                    }
                    Console.WriteLine("\nНатисніть будь-яку клавішу...");
                    Console.ReadKey();
                }
                else if (!string.IsNullOrWhiteSpace(input))
                {
                    repository.AddMessage(new Message { 
                        ConversationId = activeConversation.Id, 
                        SenderId = currentUser.Id, 
                        Text = input, 
                        Timestamp = DateTime.Now 
                    });
                }
            }
        }
        Console.Clear();
    }
}