using Microsoft.AspNetCore.SignalR;

namespace Tick_tak_toe.SignaR;

public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User.Identity.Name;
    }
}