namespace SNS.Application.Identity.Notifications.Contracts;

/// <summary>
/// Represents data transfer object for push notification delivery.
/// </summary>
/// <param name="Title">The push notification title.</param>
/// <param name="Body">The main body text of the push notification.</param>
/// <param name="ImageUrl">Optional banner image URL for push notification display.</param>
/// <param name="PushTarget">The push token or channel destination target.</param>
/// <param name="RedirectUrl">The client application navigation link URL.</param>
public sealed record PushNotificationDto(
    string Title,
    string Body,
    string? ImageUrl,
    string PushTarget,
    string RedirectUrl
);