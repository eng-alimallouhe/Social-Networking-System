using SNS.Application.Identity.Notifications.Contracts;

namespace SNS.Application.Identity.Notifications.Abstractions;

public interface IPushNotificationService
{
    Task SendAsync(PushNotificationDto notification);
}