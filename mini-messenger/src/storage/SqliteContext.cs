using Microsoft.EntityFrameworkCore;
// Переконайся, що ці "using" вказують на папки, де лежать твої класи
// Якщо твої моделі лежать прямо в папці src, то можливо `using` не потрібен
// або треба вказати твій namespace

public class SqliteContext : DbContext
{
    // Це таблиці в базі даних
    public DbSet<User> Users { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Вказуємо назву файлу бази даних. 
        // Він автоматично з'явиться в корені проєкту після першого запуску.
        options.UseSqlite("Data Source=messenger.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Тут ми можемо додати додаткові налаштування для таблиць, якщо потрібно.
        // Наприклад, налаштування зв'язків між таблицями.
        base.OnModelCreating(modelBuilder);
    }
}