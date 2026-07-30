using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Notifications.Commands.MarkAllNotificationsAsRead;

/// <summary>
/// Represents a command to mark all unread notifications as read for the authenticated user.
/// </summary>
public sealed record MarkAllNotificationsAsReadCommand() : ICommand;

