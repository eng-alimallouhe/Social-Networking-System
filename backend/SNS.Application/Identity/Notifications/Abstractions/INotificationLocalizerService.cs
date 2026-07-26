using SNS.Application.Identity.Notifications.Contracts;
using SNS.Domain.Identity.Notifications.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Notifications.Abstractions;

public interface INotificationLocalizerService
{
    NotificationContent Localize(
        NotificationType type,
        SupportedLanguage language,
        NotificationArguments arguments);
}