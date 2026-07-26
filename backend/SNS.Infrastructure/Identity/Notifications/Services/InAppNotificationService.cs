using Microsoft.AspNetCore.SignalR;
using SNS.Application.Identity.Notifications.Abstractions;
using SNS.Application.Identity.Notifications.Contracts;
using SNS.Infrastructure.Identity.Notifications.Hubs;

namespace SNS.Infrastructure.Identity.Notifications.Services;

public sealed class InAppNotificationService : IInAppNotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public InAppNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationToUserAsync(InAppNotificationDto inAppNotificationDto)
    {
        await _hubContext.Clients
            .User(inAppNotificationDto.UserId.ToString())
            .SendAsync("ReceiveNotification", inAppNotificationDto);
    }

    public async Task SendForceLogoutToUserAsync(Guid userId)
    {
        await _hubContext.Clients
            .User(userId.ToString())
            .SendAsync("ForceLogoutUser");
    }
}