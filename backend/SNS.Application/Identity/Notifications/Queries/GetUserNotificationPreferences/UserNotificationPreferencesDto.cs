namespace SNS.Application.Identity.Notifications.Queries.GetUserNotificationPreferences;

public sealed record UserNotificationPreferencesDto(
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
    bool EnableInAppNotifications);
