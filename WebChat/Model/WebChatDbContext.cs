using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace WebChat.Model;

public class WebChatDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WebChatDb;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(user => user.Id);

        modelBuilder.Entity<Chat>()
            .HasKey(chat => chat.Id);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Name)
            .IsUnique();

        modelBuilder.Entity<Chat>()
            .HasMany(chat => chat.Users)
            .WithMany(user => user.Chats)
            .UsingEntity(join => join.ToTable("ChatUser"));

        base.OnModelCreating(modelBuilder);
    }

    
}