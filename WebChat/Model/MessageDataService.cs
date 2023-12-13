using Microsoft.EntityFrameworkCore;
using WebChat.Components.Pages;

namespace WebChat.Model;

public class MessageDataService
{
    private WebChatDbContext context = new();

    public MessageDataService()
    {
        context = new WebChatDbContext();
        context.Database.EnsureCreated();
    }

    public Message SaveMessage(Message message)
    {
        context.Entry(message.User).State = EntityState.Unchanged;
        context.Entry(message.Chat).State = EntityState.Unchanged;
        context.Messages.Add(message);
        context.SaveChanges();
        return context.Messages.Single(u => u.Id == message.Id);
    }

    public List<Message> GetMessagesForChat(Guid chatId)
    {
        return context.Messages.Include(m => m.User).Where(m => m.ChatId == chatId).AsNoTracking().ToList();
    }
}
