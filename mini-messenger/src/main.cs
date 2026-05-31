using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // 1. Ініціалізація репозиторію (він сам завантажить файл при створенні)
        JsonRepository repository = new JsonRepository();

        // 2. Перевірка: створюємо базових користувачів лише якщо їх немає
        if (repository.Users.Count == 0)
        {
            repository.AddUser(new User { Id = 1, Username = "Alice" });
            repository.AddUser(new User { Id = 2, Username = "Bob" });

            Conversation commonChat = new Conversation { Id = 101, Title = "Чат Аліси та Боба" };
            commonChat.ParticipantIds.Add(1); // ID Alice
            commonChat.ParticipantIds.Add(2); // ID Bob
            repository.AddConversation(commonChat);
        }

        // Отримуємо актуальних користувачів з репозиторію
        User alice = repository.Users.First(u => u.Username == "Alice");
        User bob = repository.Users.First(u => u.Username == "Bob");

        bool isRunning = true;

        while (isRunning)
        {
            User? currentUser = null;

            // Логін
            while (currentUser == null && isRunning)
            {
                Console.Clear();
                Console.WriteLine(" ВХІД В МЕСЕНДЖЕР ");
                Console.WriteLine("1. Увійти як Alice");
                Console.WriteLine("2. Увійти як Bob");
                Console.WriteLine("-------------------------");
                Console.WriteLine("Або /quit");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine() ?? "";
                if (choice.ToLower() == "/quit") { isRunning = false; break; }
                if (choice == "1") currentUser = alice;
                else if (choice == "2") currentUser = bob;
            }

            if (!isRunning) break;

            // Вибір чату
            Conversation? activeConversation = null;
            while (activeConversation == null && isRunning)
            {
                Console.Clear();
                Console.WriteLine($"Користувач: {currentUser!.Username}");
                Console.WriteLine("Оберіть чат:");

                var userChats = repository.Conversations.Where(c => c.ParticipantIds.Contains(currentUser.Id)).ToList();
                for (int i = 0; i < userChats.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {userChats[i].Title}");
                }
                Console.WriteLine($"{userChats.Count + 1}. [Створити новий чат]");
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
                        Console.Write("Назва нового чату: ");
                        string title = Console.ReadLine() ?? "Новий чат";
                        activeConversation = new Conversation { Id = repository.Conversations.Count + 1, Title = title };
                        activeConversation.ParticipantIds.Add(currentUser.Id);
                        repository.AddConversation(activeConversation);
                    }
                }
            }

            if (activeConversation == null) continue;

            // Цикл чату
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== Чат: {activeConversation.Title} ===");
                Console.WriteLine($"Ви: {currentUser!.Username}");
                Console.WriteLine("-----------------------------------");

                foreach (var msg in repository.GetMessagesForConversation(activeConversation.Id))
                {
                    string senderName = repository.Users.FirstOrDefault(u => u.Id == msg.SenderId)?.Username ?? "Unknown";
                    Console.WriteLine($"[{msg.Timestamp:HH:mm:ss}] {senderName}: {msg.Text}");
                }

                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Напишіть повідомлення (/logout - зміна чату, /quit - вихід):");
                Console.Write("> ");

                string userInput = Console.ReadLine() ?? "";
                if (userInput.ToLower() == "/quit") { isRunning = false; break; }
                if (userInput.ToLower() == "/logout") break;

                if (!string.IsNullOrWhiteSpace(userInput))
                {
                    repository.AddMessage(new Message
                    {
                        Id = repository.Messages.Count + 1,
                        ConversationId = activeConversation.Id,
                        SenderId = currentUser.Id,
                        Text = userInput,
                        Timestamp = DateTime.Now
                    });
                }
            }
        }
        Console.Clear();
    }
}