namespace SNS.Application.Identity.Notifications.Contracts;

public sealed class NotificationTranslation
    : Dictionary<string, NotificationTemplate>
{
}

public sealed class NotificationTemplate
{
    public string Title { get; init; } = "";
    public string Body { get; init; } = "";
}