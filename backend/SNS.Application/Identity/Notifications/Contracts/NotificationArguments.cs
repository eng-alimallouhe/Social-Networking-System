namespace SNS.Application.Identity.Notifications.Contracts;

public sealed record NotificationArguments(
    string? ActorName,
    string? ActorProfilePictureUrl
);