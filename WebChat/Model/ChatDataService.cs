using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace WebChat.Model;

public class ChatDataService
{
    private readonly WebChatDbContext context = new();

    public ChatDataService()
    {
        context = new WebChatDbContext();
        context.Database.EnsureCreated();
    }

    public List<Chat> GetUserChats(Guid userId)
    {
        var chats = context.Chats
            .Include(c => c.Users).ToList();

        var userChats = chats.Where(c => c.Users
                .Any(u => u.Id == userId)).ToList();

        return userChats;
    }

    public List<User> GetUsersInChat(Guid chatId)
    {
        return context.Chats
            .Include(c => c.Users)
            .FirstOrDefault(c => c.Id == chatId)?
            .Users.ToList() ?? new List<User>();
    }

    public Chat GetChatById(Guid chatId)
    {
        return context.Chats
            .Include(chat => chat.Users)
            .FirstOrDefault(chat => chat.Id == chatId);
    }

    public void CreateChat(Chat chat)
    {
        context.Chats.Add(chat);
        foreach(var user in chat.Users)
        {
            context.Entry(user).State = EntityState.Unchanged;
        }
        context.SaveChanges();
    }

}
