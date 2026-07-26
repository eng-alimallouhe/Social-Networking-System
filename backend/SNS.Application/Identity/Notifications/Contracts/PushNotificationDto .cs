namespace SNS.Application.Identity.Notifications.Contracts;

public sealed record PushNotificationDto(
    string Title,
    string Body,
    string? ImageUrl,
    string PushTarget,
    string RedirectUrl
);