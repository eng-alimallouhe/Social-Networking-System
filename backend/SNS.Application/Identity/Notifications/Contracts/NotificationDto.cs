using SNS.Domain.Identity.Notifications.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Notifications.Contracts;

public sealed record NotificationDto(
    Guid UserId,
    Guid ProfileId,
    string RedirectUrl,
    string ActorPictureUrl,
    SupportedLanguage Language,
    NotificationSource Source,
    NotificationType Type,
    NotificationArguments Arguments
);