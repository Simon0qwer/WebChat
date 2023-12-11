
namespace WebChat.Model;

public class UserDataService
{
    public readonly WebChatDbContext context = new();


    private User? _currentUser;
    public User? CurrentUser => _currentUser;

    public UserDataService()
    {
        context = new WebChatDbContext();
        context.Database.EnsureCreated();
    }

    public void SetCurrentUser(User user)
    {
        _currentUser = user;
    }

    public List<User> GetUsers()
    {
        return context.Users.ToList();
    }

    public User? SaveUser(User user)
    {
        context.Users.Add(user);
        context.SaveChanges();
        return context.Users.SingleOrDefault(u => u.Id == user.Id);
    }

}