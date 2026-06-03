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
                Console.WriteLine("ВХІД В МЕСЕНДЖЕР");
                foreach (var u in repository.Users)
                    Console.WriteLine(u.Id + ". Увійти як " + u.Username);
                
                Console.WriteLine("-------------------------");
                Console.WriteLine("R. Зареєструвати нового користувача");
                Console.WriteLine("/quit - Вихід");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine() ?? "";
                if (choice.ToLower() == "/quit") { isRunning = false; break; }
                
                if (choice.ToLower() == "r")
                {
                    Console.Write("Введіть ім'я користувача: ");
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
                    
                    // Розумне відображення назви для чатів з 2 людьми
                    if (chat.ParticipantIds.Count == 2)
                    {
                        int otherUserId = chat.ParticipantIds.First(id => id != currentUser.Id);
                        var otherUser = repository.Users.FirstOrDefault(u => u.Id == otherUserId);
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
                    if (index <= userChats.Count)
                    {
                        activeConversation = userChats[index - 1];
                    }
                    else if (index == userChats.Count + 1)
                    {
                        Console.WriteLine("Оберіть користувача для чату:");
                        var others = repository.Users.Where(u => u.Id != currentUser.Id).ToList();
                        foreach (var u in others)
                            Console.WriteLine(u.Id + ". " + u.Username);
                        
                        Console.Write("ID: ");
                        if (int.TryParse(Console.ReadLine(), out int otherUserId))
                        {
                            var otherUser = repository.Users.FirstOrDefault(u => u.Id == otherUserId);
                            if (otherUser != null)
                            {
                                var newConv = new Conversation { Id = repository.Conversations.Count + 1, Title = "Чат з " + otherUser.Username };
                                newConv.ParticipantIds.Add(currentUser.Id);
                                newConv.ParticipantIds.Add(otherUser.Id);
                                
                                if (repository.AddConversation(newConv))
                                    activeConversation = newConv;
                                else
                                {
                                    Console.WriteLine("Помилка створення чату.");
                                    Console.ReadKey();
                                }
                            }
                        }
                    }
                }
            }

            if (activeConversation == null) continue;

            while (true)
            {
                // Для заголовка чату теж зробимо розумну назву
                string chatTitle = activeConversation.Title;
                if (activeConversation.ParticipantIds.Count == 2)
                {
                    int otherId = activeConversation.ParticipantIds.First(id => id != currentUser.Id);
                    var otherUser = repository.Users.FirstOrDefault(u => u.Id == otherId);
                    if (otherUser != null) chatTitle = "Чат з " + otherUser.Username;
                }

                Console.Clear();
                Console.WriteLine("=== " + chatTitle + " ===");
                Console.WriteLine("Ви: " + currentUser!.Username + " (/logout - вихід, /quit - вихід)");
                Console.WriteLine("-----------------------------------");

                foreach (var msg in repository.GetMessagesForConversation(activeConversation.Id))
                {
                    string senderName = repository.Users.FirstOrDefault(u => u.Id == msg.SenderId)?.Username ?? "Unknown";
                    Console.WriteLine("[" + msg.Timestamp.ToString("HH:mm:ss") + "] " + senderName + ": " + msg.Text);
                }

                Console.WriteLine("-----------------------------------");
                Console.Write("> ");

                string userInput = Console.ReadLine() ?? "";
                if (userInput.ToLower() == "/quit") { isRunning = false; break; }
                if (userInput.ToLower() == "/logout") break;

                if (!string.IsNullOrWhiteSpace(userInput))
                {
                    var newMessage = new Message
                    {
                        Id = repository.Messages.Count + 1,
                        ConversationId = activeConversation.Id,
                        SenderId = currentUser.Id,
                        Text = userInput,
                        Timestamp = DateTime.Now
                    };

                    if (!repository.AddMessage(newMessage))
                    {
                        Console.WriteLine("\nНатисніть будь-яку клавішу...");
                        Console.ReadKey();
                    }
                }
            }
        }
        Console.Clear();
    }
}