namespace WebChat.Model;

public class Chat
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
