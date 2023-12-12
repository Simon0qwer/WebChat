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
    private List<Model.Chat> chatList = new List<Model.Chat>();
    private Guid selectedUserId => userService.CurrentUser?.Id ?? Guid.Empty;
    //private User? currentUser => userList.FirstOrDefault(u => u.Id == selectedUserId);
    private User? currentUser => userService.CurrentUser;

    private Model.Chat _chat;

    [Parameter]
    public string ChatId { get; set; }
    private Guid _chatId { get; set; }

    protected override async Task OnInitializedAsync()
    {        
        userList = userService.GetUsers();
        chatList = chatService.GetChats();
        
        hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/chathub"))
            .Build();

        hubConnection.On<Message>("ReceiveMessage", async (message) =>
        {
            Console.WriteLine("Message received");
            var user = userList.FirstOrDefault(u => u.Id == message.UserId);
            if (user == null)
            {
                userList = userService.GetUsers();
                user = userList.FirstOrDefault(u => u.Id == message.UserId);
                if (user == null)
                {
                    throw new Exception($"Something went wrong, User {message.UserId} not found.");
                }
                message.User = user;
            }

            var chat = chatList.FirstOrDefault(u => u.Id == message.ChatId);
            if (chat == null)
            {
                chatList = chatService.GetChats();
                chat = chatList.FirstOrDefault(u => u.Id == message.ChatId);
                if (chat == null)
                {
                    throw new Exception($"Something went wrong, User {message.ChatId} not found.");
                }
                message.Chat = chat;
            }

            //_currentChat = chatService.CurrentChat;
            //if (_currentChat is not null)
            //{
            //    messages = chatService.GetMessagesInChat(_currentChat.Id);
            //}



            //var encodedMsg = $"{user.Name}: {message}";

            messages.Add(message);

            //var newMessage = new Message { User = user, Text = encodedMsg, Chat = chat};
           
            await InvokeAsync(StateHasChanged);
        });

        if (!string.IsNullOrWhiteSpace(ChatId))
        {
            _chatId = Guid.Parse(ChatId);
        }
        _chat = chatList.Single(c => c.Id == _chatId);

        messages = messageService.GetMessagesForChat(_chatId);

        await hubConnection.StartAsync();
    }
    private async Task Send()
    {
        if (hubConnection is not null && !string.IsNullOrWhiteSpace(messageInput))
        {
            await hubConnection.SendAsync("SendMessage", new Message { ChatId = _chatId, UserId = selectedUserId, Timestamp = DateTime.Now, Text = messageInput , User = currentUser, Chat = _chat});
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