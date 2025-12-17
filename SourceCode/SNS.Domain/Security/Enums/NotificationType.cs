
namespace SNS.Domain.Security.Enums;

public enum NotificationType
{
    // --- Content ---
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

    // --- Projects ---
    ProjectRated,
    ProjectContributorAdded,
    ProjectContributorAccepted,
    ProjectContributorRejected,
    ProjectMilestoneAdded,

    // --- Requests ---
    SkillRequestApproved,
    SkillRequestRejected,

    InterestRequestApproved,
    InterestRequestRejected,

    UniversityRequestApproved,
    UniversityRequestRejected,

    FacultyRequestApproved,
    FacultyRequestRejected,

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
