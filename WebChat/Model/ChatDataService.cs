using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WebChat.Components.Pages;

namespace WebChat.Model;

public class ChatDataService
{
    private WebChatDbContext context = new();

    private Chat? _currentChat;
    public Chat? CurrentChat => _currentChat;

    public ChatDataService()
    {
        context = new WebChatDbContext();
        context.Database.EnsureCreated();
    }
    public void SetCurrentChat(Chat chat)
    {
        _currentChat = chat;
    }

    public List<Chat> GetUserChats(Guid userId)
    {
        var chats = context.Chats
            .Include(c => c.Users).AsNoTracking().ToList();

        var userChats = chats.Where(c => c.Users
                .Any(u => u.Id == userId)).ToList();

        return userChats;
    }

    public List<User> GetUsersInChat(Guid chatId)
    {
        return context.Chats
            .Include(c => c.Users).AsNoTracking()
            .FirstOrDefault(c => c.Id == chatId)?
            .Users.ToList() ?? new List<User>();
    }

    public List<Message> GetMessagesInChat(Guid chatId)
    {
        return context.Chats
            .Include(c => c.Messages).AsNoTracking()
            .FirstOrDefault(c => c.Id == chatId)?
            .Messages.ToList() ?? new List<Message>();
    }

    public List<Chat> GetChats()
    {
        return context.Chats.AsNoTracking().ToList();
    }

    public void CreateChat(Chat chat)
    {
        foreach(var user in chat.Users)
        {
            context.Entry(user).State = EntityState.Unchanged;
        }
        context.Chats.Add(chat);
        context.SaveChanges();
    }

    public void DeleteChat(Guid chatId, List<Chat> userChats)
    {
        var chatToDelete = context.Chats
        .Include(c => c.Messages) 
        .FirstOrDefault(c => c.Id == chatId);

        if (chatToDelete != null)
        {
            context.Messages.RemoveRange(chatToDelete.Messages);
            context.Chats.Remove(chatToDelete);
            context.SaveChanges();
        }
        userChats.Remove(userChats.First(c => c.Id == chatId));
    }
    
}
