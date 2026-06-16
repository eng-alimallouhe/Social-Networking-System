using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Identity.Notifications.Entities;

public class UserNotificationPreferences: Entity, IHardDeletable
{
    public Guid Id { get; private set; } 

    public Guid UserId { get; private set; }

    // ===== Social =====
    public bool NewFollower { get; private set; }
    public bool PostLikes { get; private set; }
    public bool PostComments { get; private set; }
    public bool CommentReplies { get; private set; }
    public bool Mentions { get; private set; }
    public bool Messages { get; private set; }

    // ===== Communities =====
    public bool CommunityPosts { get; private set; }
    public bool CommunityAnnouncements { get; private set; }



    // ===== Projects =====
    public bool ProjectInvitations { get; private set; }
    public bool ProjectUpdates { get; private set; }

    // ===== Problems =====
    public bool ProblemSolutions { get; private set; }

    // ===== Security =====
    public bool LoginAlerts { get; private set; }
    public bool PasswordChanged { get; private set; }

    // ===== Delivery Channels =====
    public bool EnableEmailNotifications { get; private set; }
    public bool EnableSmsNotifications { get; private set; }
    public bool EnablePushNotifications { get; private set; }
    public bool EnableInAppNotifications { get; private set; }

    private UserNotificationPreferences()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        NewFollower = false;
        PostLikes = false;
        PostComments = false;
        CommentReplies = false;
        Mentions = false;
        Messages = false;
        CommunityPosts = false;
        CommunityAnnouncements = false;
        ProjectInvitations = false;
        ProjectUpdates = false;
        ProblemSolutions = false;
        LoginAlerts = false;
        PasswordChanged = false;
        EnableEmailNotifications = false;
        EnableSmsNotifications = false;
        EnablePushNotifications = false;
        EnableInAppNotifications = false;
    }

    public static UserNotificationPreferences Create(Guid userId)
    {
        var preferences = new UserNotificationPreferences
        {
            UserId = userId
        };
        return preferences;
    }

    // أضف هذه الدوال التكتيكية داخل كلاس UserNotificationPreferences في الدومين:

    public void UpdateSocialPreferences(bool newFollower, bool postLikes, bool postComments, bool commentReplies, bool mentions, bool messages)
    {
        NewFollower = newFollower;
        PostLikes = postLikes;
        PostComments = postComments;
        CommentReplies = commentReplies;
        Mentions = mentions;
        Messages = messages;
    }

    public void UpdateCommunityPreferences(bool communityPosts, bool communityAnnouncements)
    {
        CommunityPosts = communityPosts;
        CommunityAnnouncements = communityAnnouncements;
    }

    public void UpdateProjectPreferences(bool projectInvitations, bool projectUpdates)
    {
        ProjectInvitations = projectInvitations;
        ProjectUpdates = projectUpdates;
    }

    public void UpdateProblemPreferences(bool problemSolutions)
    {
        ProblemSolutions = problemSolutions;
    }

    public void UpdateSecurityPreferences(bool loginAlerts, bool passwordChanged)
    {
        LoginAlerts = loginAlerts;
        PasswordChanged = passwordChanged;
    }

    public void UpdateDeliveryChannels(bool email, bool sms, bool push, bool inApp)
    {
        EnableEmailNotifications = email;
        EnableSmsNotifications = sms;
        EnablePushNotifications = push;
        EnableInAppNotifications = inApp;
    }
}
