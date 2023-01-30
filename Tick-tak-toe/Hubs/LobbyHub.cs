using Microsoft.AspNetCore.SignalR;

namespace Tick_tak_toe.Hubs;

public class LobbyHub : Hub
{
    public int UsersInLobby { get; set; } = 0;

    public override Task OnConnectedAsync()
    {
        UsersInLobby++;
        Clients.All.SendAsync("updateOnlineUser").GetAwaiter().GetResult();
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        UsersInLobby--;
        Clients.All.SendAsync("updateOnlineUser").GetAwaiter().GetResult();
        return base.OnDisconnectedAsync(exception);
    }

    public async Task CreatedNewRoom()
    {
        await Clients.All.SendAsync("createRoom");
    }
}