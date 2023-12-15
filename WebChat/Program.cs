using Microsoft.AspNetCore.ResponseCompression;
using WebChat.Hubs;
using WebChat.Components;
using WebChat.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents().AddCircuitOptions(o =>
    {
        o.DetailedErrors = true;
    });

// Configure response compression
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddScoped<UserDataService>();
builder.Services.AddScoped<ChatDataService>();
builder.Services.AddScoped<MessageDataService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Use response compression middleware
app.UseResponseCompression();

app.MapHub<ChatHub>("/chathub");
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();

// gitkraken token
// eJwtzE2PwiAUheH/wroL6IcV1rOaRDPJuOlsCJRbxUIhl9aOGv+7xHR3cpLnfRLV95DSKYwwEUGoanXFmso0BkrGh5IpXrYN53RfVbrVdU2betCUFCRiuFkDuNHnVpLzljpfgjywweA1/f5xyy5L2vnjz+OrO+3b6Ff9TbnufJdTHyLne4TsNCgEzG/qw+dQxttJxEU728sR7gWCMiLgOY8YiiUBCvDKumINOA4urBnDf7QISaqZiGlx7vV6A/7mT84=