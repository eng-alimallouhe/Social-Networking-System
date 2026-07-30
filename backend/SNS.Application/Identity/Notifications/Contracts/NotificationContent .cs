namespace SNS.Application.Identity.Notifications.Contracts;

/// <summary>
/// Represents formatted notification text content containing title and body string.
/// </summary>
/// <param name="Title">The formatted notification title text.</param>
/// <param name="Body">The formatted notification body message text.</param>
public sealed record NotificationContent(
    string Title,
    string Body
);

