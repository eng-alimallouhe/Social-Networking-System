namespace SNS.Domain.Identity.Notifications.Enums;

public enum NotificationType
{
    // --- ContentManagement ---
    PostCreated,
    PostLiked,
    PostReacted,
    PostCommented,
    CommentReplied,
    
    // --- Community ---
    CommunityJoinRequestApproved,
    CommunityJoinRequestRejected,
    CommunityInvitationReceived,
    CommunityInvitationAccepted,
    CommunityInvitationRejected,
    CommunityRoleChanged,
    CommunityRulesUpdated,

    // --- Problems & Solutions ---
    ProblemAnswered,
    SolutionAccepted,
    ProblemUpvoted,
    SolutionUpvoted,

    // --- Social Graph ----
    Follow,

    // --- Projects ---
    ProjectRated,
    ProjectContributorAdded,
    ProjectContributorAccepted,
    ProjectContributorRejected,
    ProjectMilestoneAdded,



    // --- System ---
    NewAnnouncement,
    AccountVerified,
    AccountSuspended,

    // --- Security ---
    NewLogin,
    PasswordChanged,
    TwoFactorEnabled,
    TwoFactorDisabled,
}
