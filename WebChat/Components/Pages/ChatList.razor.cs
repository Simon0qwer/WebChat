using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Text;
using WebChat.Model;

namespace WebChat.Components.Pages;

partial class ChatList
{
    private List<Model.Chat> userChats = new List<Model.Chat>();
    private List<User> usersInNewChat = new List<User>();
    private List<User> userList = new List<User>();

    private Guid selectedUserForNewChat = Guid.Empty;
    private Guid selectedUserId = Guid.Empty;

    private string newChatName = string.Empty;
    private Guid selectedChatIdForSettings = Guid.Empty;
    private Guid userInChatId;
    private bool showNewChatBox = false;
    private bool showChatSettings = false;

    private User? _currentUser;

    protected override void OnInitialized()
    {
        _currentUser = userService.CurrentUser;
        if(_currentUser is not null)
        {
            userChats = chatService.GetUserChats(_currentUser.Id);
        }
        userList = userService.GetUsers();
    }

    private void CreateNewChat()
    {
        var newChat = new Model.Chat
        {
            Id = Guid.NewGuid(),
            Name = newChatName,
            Users = usersInNewChat,
        };

        chatService.CreateChat(newChat);

        if (_currentUser is not null)
        {
            userChats = chatService.GetUserChats(_currentUser.Id);
        }

        newChatName = string.Empty;
    }
    private void AddUserToNewChat(Guid userId)
    {
        if (userId != Guid.Empty && !usersInNewChat.Any(u => u.Id == userId))
        {
            var selectedUser = userList.FirstOrDefault(u => u.Id == userId);
            if (selectedUser != null)
            {
                usersInNewChat.Add(selectedUser);
            }
        }
    }

    private void AddUserToChat(Guid chatId, Guid userId)
    {
        var usersInChat = chatService.GetUsersInChat(chatId);

        if (userId != Guid.Empty && !usersInChat.Any(u => u.Id == userId))
        {
            var selectedUser = userList.FirstOrDefault(u => u.Id == userId);
            if (selectedUser != null)
            {
                chatService.AddUserToChat(chatId, selectedUser.Id);
                userChats = chatService.GetUserChats(_currentUser.Id);
                //userChats[userChats.IndexOf(userChats.Single(c => c.Id == chatId))] =  chatService.AddUserToChat(chatId, selectedUserId);
            }
        }

    }

    private void RemoveUserFromChat(Guid chatId, Guid userId)
    {
        var usersInChat = chatService.GetUsersInChat(chatId);

        if (userId != Guid.Empty && usersInChat.Any(u => u.Id == userId))
        {
            var selectedUser = userList.FirstOrDefault(u => u.Id == userId);
            if (selectedUser != null)
            {
                chatService.RemoveUserFromChat(chatId, selectedUser.Id);
                userChats = chatService.GetUserChats(_currentUser.Id);
            }
        }
    }


    private void ToggleNewChatBox()
    {
        showNewChatBox = !showNewChatBox;

        // Reset the new chat form when closing the new chat box
        if (!showNewChatBox)
        {
            newChatName = string.Empty;
            usersInNewChat.Clear();
            userInChatId = Guid.Empty;
        }
    }

    private void ToggleChatSettings(Guid chatId)
    {
        showChatSettings = !showChatSettings;
        if (showNewChatBox)
        {
            showNewChatBox = !showNewChatBox;
        }
        selectedChatIdForSettings = chatId;
    }

    protected void NavigateToChat(Guid chatId)
    {        
        Navigation.NavigateTo($"/chat/{chatId}");
        
    }
       
}