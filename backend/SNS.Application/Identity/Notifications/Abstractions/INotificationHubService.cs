namespace SNS.Application.Identity.Notifications.Abstractions;

public interface INotificationHubService
{
    Task SendNotificationToUserAsync(Guid userId, object notificationDto);

    Task SendForceLogoutToUserAsync(Guid userId);
}
