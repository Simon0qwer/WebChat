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
    private string registrationPassword = "";

    private void Login()
    {
        showError = false;

        if (!string.IsNullOrWhiteSpace(registrationName) && !string.IsNullOrWhiteSpace(registrationPassword))
        {
            var userWithSameName = userService.GetUsers().FirstOrDefault(user => user.Name == registrationName);

            if (userWithSameName != null && userWithSameName.Password == registrationPassword)
            {
                userService.SetCurrentUser(userWithSameName);
                Navigation.NavigateTo("/chats", false);
            }
            else
            {
                registrationName = "";
                showError = true;
            }
        }
    }

    private void Register()
    {
        showError = false;
        if (!string.IsNullOrWhiteSpace(registrationName) && !string.IsNullOrWhiteSpace(registrationPassword))
        {
            if (userService.GetUsers().Any(user => user.Name == registrationName))
            {
                registrationName = "";
                showError = true;
                return;
            }

            var newUser = new User { Name = registrationName, Id = Guid.NewGuid(), Password = registrationPassword};
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
