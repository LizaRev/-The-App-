using System;

class Program
{
    static void Main(string[] args)
    {
        JsonRepository repository = new JsonRepository();
       
        User alice = new User { Id = 1, Username = "Alice" };
        User bob = new User { Id = 2, Username = "Bob" };

        repository.AddUser(alice);
        repository.AddUser(bob);

        // 3. Створюємо спільний чат
        Conversation conversation = new Conversation 
        { 
            Id = 101, 
            Title = "Загальний чат" 
        };
        
        conversation.ParticipantIds.Add(alice.Id);
        conversation.ParticipantIds.Add(bob.Id);
        repository.Conversations.Add(conversation);

        bool isRunning = true;
        
        while (isRunning)
        {
            User currentUser = null;

            while (currentUser == null)
            {
                Сonsole.Clear();
                Console.WriteLine(" ВХІД В МЕСЕНДЖЕР ");
                Console.WriteLine("Оберіть користувача:");
                Console.WriteLine("1. Увійти як Alice");
                Console.WriteLine("2. Увійти як Bob");
                Console.WriteLine("-------------------------");
                Console.WriteLine("Або напишіть /quit для виходу");
                Console.Write("Ваш вибір: ");
                
                string choice = Console.ReadLine();

                if (choice?.ToLower() == "/quit")
                {
                    isRunning = false;
                    break; 
                }
                else if (choice == "1")
                {
                    currentUser = alice;
                }
                else if (choice == "2")
                {
                    currentUser = bob;
                }
                else
                {
                    Console.WriteLine("Невірний вибір! Натисніть Enter, щоб спробувати знову");
                    Console.ReadLine();
                }
            }

            if (!isRunning) break;

            while (true)
            {
                Console.Clear();
                
                string chatPartner = (currentUser.Id == alice.Id) ? bob.Username : alice.Username;
                
                Console.WriteLine($"=== Чат: {chatPartner} ===");
                Console.WriteLine($"Ви увійшли як: {currentUser.Username}");
                Console.WriteLine("-----------------------------------");

                // Витягуємо повідомлення
                var chatMessages = repository.GetMessagesForConversation(101);

                // Виводимо історію
                foreach (var msg in chatMessages)
                {
                    string senderName = (msg.SenderId == alice.Id) ? alice.Username : bob.Username;
                    Console.WriteLine($"[{msg.Timestamp:HH:mm:ss}] {senderName}: {msg.Text}");
                }

                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Напишіть повідомлення (або /logout щоб змінити акаунт, /quit для виходу):");
                Console.Write("> ");

                string userInput = Console.ReadLine();

                if (userInput?.ToLower() == "/quit")
                {
                    isRunning = false;
                    break; 
                }

                if (userInput?.ToLower() == "/logout")
                {
                    break; 
                }

                // Відправка повідомлення
                if (!string.IsNullOrWhiteSpace(userInput))
                {
                    Message newMsg = new Message
                    {
                        Id = repository.Messages.Count + 1,
                        ConversationId = 101,
                        SenderId = currentUser.Id,
                        Text = userInput,
                        Timestamp = DateTime.Now
                    };

                    repository.AddMessage(newMsg);
                }
            }
        }

        // Прибрали фінальний текст, просто очищаємо екран при виході
        Console.Clear();
    }
}