namespace SNS.Application.Identity.Notifications.Contracts;

/// <summary>
/// Represents contextual parameter arguments used to construct and format notification templates.
/// </summary>
/// <param name="ActorName">The display name of the actor who performed the action.</param>
/// <param name="ActorProfilePictureUrl">Optional profile picture URL of the actor.</param>
public sealed record NotificationArguments(
    string? ActorName,
    string? ActorProfilePictureUrl
);