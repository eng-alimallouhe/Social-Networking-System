using SNS.Domain.Identity.Notifications.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Notifications.Contracts;

/// <summary>
/// Represents data transfer object containing notification details to be dispatched or persisted.
/// </summary>
/// <param name="UserId">The recipient user identifier.</param>
/// <param name="ProfileId">The recipient profile identifier.</param>
/// <param name="RedirectUrl">The target web redirect URL associated with the notification action.</param>
/// <param name="ActorPictureUrl">The profile picture URL of the actor triggering the notification.</param>
/// <param name="Language">The supported language for notification message rendering.</param>
/// <param name="Source">The domain source classification of the notification.</param>
/// <param name="Type">The specific type classification of the notification event.</param>
/// <param name="Arguments">Contextual arguments for formatting the notification message.</param>
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