using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Notifications.Commands.MarkSingleNotificationAsRead;

public sealed record MarkSingleNotificationAsReadCommand(
    Guid NotificationId) : ICommand;
