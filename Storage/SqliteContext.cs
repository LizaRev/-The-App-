using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MessengerGui.Models; // Важливо для бачення User, Message, Conversation

namespace MessengerGui.Storage
{
    public class SqliteContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=messenger.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}