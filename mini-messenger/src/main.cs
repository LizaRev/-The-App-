using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        JsonRepository repository = new JsonRepository();
        bool isRunning = true;

        while (isRunning)
        {
            User? currentUser = null;

            while (currentUser == null && isRunning)
            {
                Console.Clear();
                Console.WriteLine("=== ВХІД В МЕСЕНДЖЕР ===");
                foreach (var u in repository.Users)
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
                        int newId = repository.Users.Count > 0 ? repository.Users.Max(u => u.Id) + 1 : 1;
                        repository.AddUser(new User { Id = newId, Username = name });
                        Console.WriteLine("Користувача створено!");
                        Console.ReadKey();
                    }
                }
                else if (int.TryParse(choice, out int userId))
                    currentUser = repository.Users.FirstOrDefault(u => u.Id == userId);
            }

            if (!isRunning) break;

            Conversation? activeConversation = null;
            while (activeConversation == null && isRunning)
            {
                Console.Clear();
                Console.WriteLine("Користувач: " + currentUser!.Username);
                Console.WriteLine("Оберіть чат:");

                var userChats = repository.Conversations.Where(c => c.ParticipantIds.Contains(currentUser.Id)).ToList();
                for (int i = 0; i < userChats.Count; i++)
                {
                    var chat = userChats[i];
                    string displayName = chat.Title;
                    var otherId = chat.ParticipantIds.FirstOrDefault(id => id != currentUser.Id);
                    if (otherId != 0)
                    {
                        var otherUser = repository.Users.FirstOrDefault(u => u.Id == otherId);
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
                        foreach (var u in repository.Users.Where(u => u.Id != currentUser.Id))
                            Console.WriteLine(u.Id + ". " + u.Username);
                        
                        if (int.TryParse(Console.ReadLine(), out int otherId))
                        {
                            var otherUser = repository.Users.FirstOrDefault(u => u.Id == otherId);
                            if (otherUser != null)
                            {
                                var newConv = new Conversation { Id = repository.Conversations.Count + 1, Title = "Чат" };
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
                    var otherUser = repository.Users.FirstOrDefault(u => u.Id == otherId);
                    if (otherUser != null) chatTitle = "Чат з " + otherUser.Username;
                }

                Console.Clear();
                Console.WriteLine("=== " + chatTitle + " ===");
                Console.WriteLine("ДОПОМОГА: /search [текст] - знайти смс | /logout - вийти з чату | /quit - закрити програму");
                Console.WriteLine("-----------------------------------");

                foreach (var msg in repository.GetMessagesForConversation(activeConversation.Id))
                {
                    var sender = repository.Users.FirstOrDefault(u => u.Id == msg.SenderId);
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
                        Console.WriteLine($"\n--- Результати пошуку '{query}' ---");
                        var results = repository.Messages.Where(m => m.Text.Contains(query, StringComparison.OrdinalIgnoreCase));
                        foreach (var m in results)
                            Console.WriteLine($"[{m.Timestamp:HH:mm}] {m.Text}");
                    }
                    else
                    {
                        Console.WriteLine("Помилка: Введіть текст після /search");
                    }
                    Console.WriteLine("\nНатисніть будь-яку клавішу...");
                    Console.ReadKey();
                }
                else if (!string.IsNullOrWhiteSpace(input))
                {
                    repository.AddMessage(new Message { 
                        Id = repository.Messages.Count + 1, 
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