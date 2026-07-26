using SNS.Application.Identity.Notifications.Contracts;

namespace SNS.Application.Identity.Notifications.Abstractions;

public interface IInAppNotificationService
{
    Task SendNotificationToUserAsync(InAppNotificationDto inAppNotificationDto);

    Task SendForceLogoutToUserAsync(Guid userId);
}