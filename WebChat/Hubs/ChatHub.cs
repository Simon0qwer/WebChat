using Microsoft.AspNetCore.SignalR;
using WebChat.Model;

namespace WebChat.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(User user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user.Id, message);
    }
}