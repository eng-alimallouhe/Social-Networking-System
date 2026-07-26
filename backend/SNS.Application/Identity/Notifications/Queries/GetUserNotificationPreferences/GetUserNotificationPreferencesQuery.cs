using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Identity.Notifications.Queries.GetUserNotificationPreferences;

public sealed record GetUserNotificationPreferencesQuery() : IQuery<UserNotificationPreferencesDto>;
