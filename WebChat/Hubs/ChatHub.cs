using Microsoft.AspNetCore.SignalR;
using WebChat.Model;

namespace WebChat.Hubs;

public class ChatHub(MessageDataService messageService) : Hub
{
    private readonly MessageDataService messageService = messageService;

    public async Task SendMessage(Message message)
    {
        message = messageService.SaveMessage(message);
        message.User = null;
        message.Chat = null;
        await Clients.All.SendAsync("ReceiveMessage", message);

    }
}