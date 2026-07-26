using SNS.Application.Identity.Notifications.Contracts;

namespace SNS.Application.Identity.Notifications.Abstractions;

public interface INotificationDeliveryService
{
    Task SendAsync(NotificationDto notificationDto);
}