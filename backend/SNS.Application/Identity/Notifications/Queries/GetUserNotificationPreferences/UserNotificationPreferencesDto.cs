namespace SNS.Application.Identity.Notifications.Queries.GetUserNotificationPreferences;

/// <summary>
/// Represents response DTO containing user notification preferences and channel configurations.
/// </summary>
/// <param name="NewFollower">Flag indicating whether notifications are enabled for new followers.</param>
/// <param name="PostLikes">Flag indicating whether notifications are enabled for post likes.</param>
/// <param name="PostComments">Flag indicating whether notifications are enabled for post comments.</param>
/// <param name="CommentReplies">Flag indicating whether notifications are enabled for comment replies.</param>
/// <param name="Mentions">Flag indicating whether notifications are enabled for user mentions.</param>
/// <param name="Messages">Flag indicating whether notifications are enabled for direct messages.</param>
/// <param name="CommunityPosts">Flag indicating whether notifications are enabled for community posts.</param>
/// <param name="CommunityAnnouncements">Flag indicating whether notifications are enabled for community announcements.</param>
/// <param name="ProjectInvitations">Flag indicating whether notifications are enabled for project invitations.</param>
/// <param name="ProjectUpdates">Flag indicating whether notifications are enabled for project updates.</param>
/// <param name="ProblemSolutions">Flag indicating whether notifications are enabled for problem solution submissions.</param>
/// <param name="LoginAlerts">Flag indicating whether notifications are enabled for login security alerts.</param>
/// <param name="PasswordChanged">Flag indicating whether notifications are enabled for password changes.</param>
/// <param name="EnableEmailNotifications">Flag indicating whether email notifications delivery is enabled.</param>
/// <param name="EnableSmsNotifications">Flag indicating whether SMS notifications delivery is enabled.</param>
/// <param name="EnablePushNotifications">Flag indicating whether push notifications delivery is enabled.</param>
/// <param name="EnableInAppNotifications">Flag indicating whether in-app notifications delivery is enabled.</param>
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

