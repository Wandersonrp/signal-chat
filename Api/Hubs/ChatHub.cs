using Microsoft.AspNetCore.SignalR;
using SignalChat.Api.Data;
using SignalChat.Api.Services.Users;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace SignalChat.Api.Hubs;

public class ChatHub : Hub
{
    private readonly ConnectionManager _connectionManager;
    private readonly IUserService _userService;

    public ChatHub(ConnectionManager connectionManager, IUserService userService)
    {
        _connectionManager = connectionManager;
        _userService = userService;
    }

    public async Task InitializeUserList()
    {
        var list = _connectionManager.GetUsers();

        await Clients.All.SendAsync("ReceiveInitializeUserList", list);
    }

    public void AddUserToRoom()
    {
        var currentUser = Context.User?
            .Claims
            .Where(claim => claim.Type == ClaimTypes.Email)?
            .FirstOrDefault()?
            .Value;

        var connectionId = GetConnectionId();

        _connectionManager.Add(currentUser ?? string.Empty, connectionId);
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var connectionId = GetConnectionId();

            var userEmail = Context.User?
                .Claims
                .Where(claim => claim.Type == ClaimTypes.Email)?
                .FirstOrDefault()?
                .Value;

            var userName = Context.User?
                .Claims
                .Where(claim => claim.Type == ClaimTypes.Name)?
                .FirstOrDefault()?
                .Value;

            if(userEmail is not null)
            {
                var connectionKey = $"{userEmail}-{userName}";

                _connectionManager.Add(connectionKey, connectionId);
            }
            
            await base.OnConnectedAsync();
        }
        catch
        {
            await base.OnConnectedAsync();
        }
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userEmail = Context.User?
            .Claims
            .Where(claim => claim.Type == ClaimTypes.Email)?
            .FirstOrDefault()?
            .Value;

        var userName = Context.User?
            .Claims
            .Where(claim => claim.Type == ClaimTypes.Name)?
            .FirstOrDefault()?
            .Value;

        if(userEmail is not null)
        {
            var connectionKey = $"{userEmail}-{userName}";

            var connectionId = GetConnectionId();

            _connectionManager.Remove(connectionKey, connectionId);

            await Clients.All.SendAsync("UserDisconnected", userEmail);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    private string GetConnectionId() => Context.ConnectionId;
}
