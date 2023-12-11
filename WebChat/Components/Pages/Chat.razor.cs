using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using WebChat.Model;

namespace WebChat.Components.Pages;

partial class Chat
{
    private HubConnection? hubConnection;

    private List<Message> messages = new List<Message>();
    private string? messageInput;
    private List<User> userList = new List<User>();


    private Guid selectedUserId => userService.CurrentUser?.Id ?? Guid.Empty;
    private User? currentUser => userList.FirstOrDefault(u => u.Id == selectedUserId);
    private Model.Chat? _currentChat;

    [Parameter]
    public string ChatId { get; set; }
    private Guid _chatId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _currentChat = chatService.CurrentChat;
        if (_currentChat is not null)
        {
            messages = chatService.GetMessagesInChat(_currentChat.Id);
        }


        hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/chathub"))
            .Build();

        hubConnection.On<string, string>("ReceiveMessage", (userId, message) =>
        {
            userService.GetUsers();
            var user = userList.FirstOrDefault(u => u.Id == Guid.Parse(userId));
            if (user == null)
            {
                userList = userService.GetUsers();
                user = userList.FirstOrDefault(u => u.Id == Guid.Parse(userId));
                if (user == null)
                {
                    throw new Exception($"Something went wrong, User {userId} not found.");
                }
            }
            var encodedMsg = $"{user.Name}: {message}";

            var newMessage = new Message { User = user, Text = encodedMsg };
            messages.Add(newMessage);
            InvokeAsync(StateHasChanged);
        });

        userList = userService.GetUsers();
        if (!string.IsNullOrWhiteSpace(ChatId))
        {
            _chatId = Guid.Parse(ChatId);
        }

        await hubConnection.StartAsync();
    }

    private async Task Send()
    {
        if (hubConnection is not null)
        {
            await hubConnection.SendAsync("SendMessage", currentUser, messageInput);
        }
    }

    public bool IsConnected =>
        hubConnection?.State == HubConnectionState.Connected;

    public async ValueTask DisposeAsync()
    {        
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
    }

}