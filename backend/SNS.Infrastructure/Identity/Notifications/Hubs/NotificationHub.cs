using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SNS.Application.Identity.Shared.Abstractions;

namespace SNS.Infrastructure.Identity.Notifications.Hubs;


[Authorize]
public class NotificationHub: Hub
{
    private readonly IOnlineUserTracker _userTracker;

    public NotificationHub(IOnlineUserTracker userTracker)
    {
        _userTracker = userTracker;
    }

    public override async Task OnConnectedAsync()
    {
        if (!Guid.TryParse(Context.UserIdentifier, out var userId))
        {
            Context.Abort();
            return;
        }

        _userTracker.Connect(userId, Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _userTracker.Disconnect(base.Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}