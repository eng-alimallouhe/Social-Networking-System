using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Notifications.Commands.MarkSingleNotificationAsRead;

/// <summary>
/// Represents a command to mark a specific notification as read for the authenticated user.
/// </summary>
/// <param name="NotificationId">The unique identifier of the notification to mark as read.</param>
public sealed record MarkSingleNotificationAsReadCommand(
    Guid NotificationId) : ICommand;

