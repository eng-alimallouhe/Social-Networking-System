namespace SNS.Application.Identity.Notifications.Contracts;

public sealed record InAppNotificationDto(
    Guid UserId,
    string Title,
    string Body,
    string? TargetProfilePictureUrl
);