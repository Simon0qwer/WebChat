namespace WebChat.Model;

public class MessageDataService
{
    private readonly WebChatDbContext context = new();

    public MessageDataService()
    {
        context = new WebChatDbContext();
        context.Database.EnsureCreated();
    }
}
