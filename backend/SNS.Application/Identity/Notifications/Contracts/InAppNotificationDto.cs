namespace SNS.Application.Identity.Notifications.Contracts;

/// <summary>
/// Represents data transfer object for in-app real-time notification delivery.
/// </summary>
/// <param name="UserId">The recipient user identifier.</param>
/// <param name="Title">The notification title header.</param>
/// <param name="Body">The notification body text content.</param>
/// <param name="TargetProfilePictureUrl">Optional profile avatar URL of the originating actor.</param>
public sealed record InAppNotificationDto(
    Guid UserId,
    string Title,
    string Body,
    string? TargetProfilePictureUrl
);