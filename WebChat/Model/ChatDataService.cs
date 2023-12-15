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

    //public void UpdateChat(Guid chatId, List<Guid> users)
    //{
    //    Chat chat = context.Chats.Find(chatId);
    //    context.Chats.Update(chat);
    //    context.Users.Update()
    //}

    public Chat? AddUserToChat(Guid chatId, Guid userId)
    {
        var chat = context.Chats.Include(c => c.Users).FirstOrDefault(c => c.Id == chatId);
        var user = context.Users.Include(u => u.Chats).FirstOrDefault(u => u.Id == userId);

        if (chat != null && user != null && !chat.Users.Any(u => u.Id == userId))
        {
            chat.Users.Add(user);   
            context.Entry(chat).State = EntityState.Modified;
            user.Chats.Add(chat);
            context.Entry(user).State = EntityState.Modified;


            context.SaveChanges();
        }
        return chat;
    }

    public Chat? RemoveUserFromChat(Guid chatId, Guid userId)
    {
        var chat = context.Chats.Include(c => c.Users).FirstOrDefault(c => c.Id == chatId);
        var user = context.Users.Include(u => u.Chats).FirstOrDefault(u => u.Id == userId);

        if (chat != null && user != null && chat.Users.Any(u => u.Id == userId))
        {
            chat.Users.Remove(user);
            context.Entry(chat).State = EntityState.Modified;
            user.Chats.Remove(chat);
            context.Entry(user).State = EntityState.Modified;
            
            
            context.SaveChanges();
        }
        return chat;
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
