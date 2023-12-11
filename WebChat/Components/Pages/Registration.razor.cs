using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using WebChat.Model;

namespace WebChat.Components.Pages;

partial class Registration
{
    private bool showMenu = true;
    private bool showRegistration = false;
    private bool showLogin = false;
    private bool showError;
    private string registrationName = "";

    private void Login()
    {
        showError = false;
        if (!string.IsNullOrWhiteSpace(registrationName))
        {
            if (!userService.GetUsers().Any(user => user.Name == registrationName))
            {
                registrationName = "";
                showError = true;
                return;
            }

            var existingUser = userService.GetUsers().First(user => user.Name == registrationName);
            userService.SetCurrentUser(existingUser);
            Navigation.NavigateTo("/chats", false);
        }
    }

    private void Register()
    {
        showError = false;
        if (!string.IsNullOrWhiteSpace(registrationName))
        {
            if (userService.GetUsers().Any(user => user.Name == registrationName))
            {
                registrationName = "";
                showError = true;
                return;
            }

            var newUser = new User { Name = registrationName, Id = Guid.NewGuid() };
            var savedUser = userService.SaveUser(newUser);
            if(savedUser != null)
            {
                userService.SetCurrentUser(savedUser);
                Navigation.NavigateTo("/chats", false);
            }
            else
            {
                showError = true;
            }

        }
    }

}
