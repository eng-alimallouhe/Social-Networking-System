using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Notifications.Commands.UpdateNotificationPreferences;

/// <summary>
/// Represents a command to update notification settings and delivery channel preferences for the authenticated user.
/// </summary>
/// <param name="NewFollower">Flag to enable notifications for new followers.</param>
/// <param name="PostLikes">Flag to enable notifications for post likes.</param>
/// <param name="PostComments">Flag to enable notifications for post comments.</param>
/// <param name="CommentReplies">Flag to enable notifications for comment replies.</param>
/// <param name="Mentions">Flag to enable notifications for user mentions.</param>
/// <param name="Messages">Flag to enable notifications for direct messages.</param>
/// <param name="CommunityPosts">Flag to enable notifications for community posts.</param>
/// <param name="CommunityAnnouncements">Flag to enable notifications for community announcements.</param>
/// <param name="ProjectInvitations">Flag to enable notifications for project invitations.</param>
/// <param name="ProjectUpdates">Flag to enable notifications for project updates.</param>
/// <param name="ProblemSolutions">Flag to enable notifications for problem solution submissions.</param>
/// <param name="LoginAlerts">Flag to enable notifications for login security alerts.</param>
/// <param name="PasswordChanged">Flag to enable notifications for password change events.</param>
/// <param name="EnableEmailNotifications">Flag to enable email delivery channel.</param>
/// <param name="EnableSmsNotifications">Flag to enable SMS delivery channel.</param>
/// <param name="EnablePushNotifications">Flag to enable push notifications delivery channel.</param>
/// <param name="EnableInAppNotifications">Flag to enable in-app notifications delivery channel.</param>
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

