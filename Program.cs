using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using SignalChat.Components;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.Name = "SignalChatCookie";
        options.LoginPath = "/auth/google-login";
    })
    .AddGoogle(options =>
    {
        var googleAuthSection = builder.Configuration.GetSection(key: "Authentication:Google");
        options.ClientId = googleAuthSection[key: "ClientId"] ?? throw new ArgumentNullException("Google Authentication Client Id is null");
        options.ClientSecret = googleAuthSection[key: "ClientSecret"] ?? throw new ArgumentNullException("Google Authentication Client Secret is null");

        options.Scope.Add("Profile");
        options.Events.OnCreatingTicket = context =>
        {
            var pictureUri = context.User.GetProperty("picture").GetString() ?? string.Empty;

            context.Identity.AddClaim(new Claim("picture", pictureUri));
            return Task.CompletedTask;
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthorization();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
  