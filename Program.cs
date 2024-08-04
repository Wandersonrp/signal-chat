using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using SignalChat;
using SignalChat.Api.Data;
using SignalChat.Api.Hubs;
using SignalChat.Api.Services.Users;
using SignalChat.Components;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<CookiesProvider>();
builder.Services.AddSingleton<ConnectionManager>();

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

builder.Services.AddDbContext<SignalChatDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR(config => config.EnableDetailedErrors = true);

builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] 
    {  
        "application/octet-stream"
    });
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

app.MapHub<ChatHub>("/chat");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
  