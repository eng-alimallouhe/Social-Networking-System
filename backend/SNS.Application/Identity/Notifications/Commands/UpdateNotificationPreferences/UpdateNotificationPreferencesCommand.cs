using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Notifications.Commands.UpdateNotificationPreferences;

public sealed record UpdateNotificationPreferencesCommand(
    bool NewFollower,
    bool PostLikes,
    bool PostComments,
    bool CommentReplies,
    bool Mentions,
    bool Messages,
    bool CommunityPosts,
    bool CommunityAnnouncements,
    bool ProjectInvitations,
    bool ProjectUpdates,
    bool ProblemSolutions,
    bool LoginAlerts,
    bool PasswordChanged,
    bool EnableEmailNotifications,
    bool EnableSmsNotifications,
    bool EnablePushNotifications,
    bool EnableInAppNotifications) : ICommand;
