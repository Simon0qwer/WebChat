namespace WebChat.Model;

public class MessageDataService
{
    private readonly WebChatDbContext context = new();

    public MessageDataService()
    {
        context = new WebChatDbContext();
        context.Database.EnsureCreated();
    }

    public Message? SaveMessage(Message message)
    {
        context.Messages.Add(message);
        context.SaveChanges();
        return context.Messages.SingleOrDefault(u => u.Id == message.Id);
    }

}
