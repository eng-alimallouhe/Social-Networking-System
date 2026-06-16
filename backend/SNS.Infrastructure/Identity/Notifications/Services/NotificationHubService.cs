using Microsoft.AspNetCore.SignalR;
using SNS.Application.Identity.Notifications.Abstractions;
using SNS.Infrastructure.Identity.Notifications.Hubs;

namespace SNS.Infrastructure.Identity.Notifications.Services;

public sealed class NotificationHubService : INotificationHubService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationHubService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationToUserAsync(Guid userId, object notificationDto)
    {
        await _hubContext.Clients
            .User(userId.ToString())
            .SendAsync("ReceiveNotification", notificationDto);
    }

    public async Task SendForceLogoutToUserAsync(Guid userId)
    {
        await _hubContext.Clients
            .User(userId.ToString())
            .SendAsync("ForceLogoutUser");
    }
}