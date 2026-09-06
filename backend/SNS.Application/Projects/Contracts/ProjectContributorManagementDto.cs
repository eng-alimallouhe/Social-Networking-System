using SNS.Domain.Projects.Enums;

namespace SNS.Application.Projects.Contracts;

public record ProjectContributorManagementDto(
    Guid ContributorRecordId,
    Guid ProfileId,
    string? ProfileImageUrl,
    string DisplayName,
    string? Specialization,
    int FollowersCount,
    int FollowingCount,
    bool IsFollowedByCurrentUser,
    string Role,
    InvitingStatus InvitingStatus,
    DateTime InvitationSentAt,
    DateTime? RespondedAt,
    string? InvitationMessage
);
