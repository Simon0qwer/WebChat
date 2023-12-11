namespace WebChat.Model;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<Chat> Chats { get; set; } = new List<Chat>();
}