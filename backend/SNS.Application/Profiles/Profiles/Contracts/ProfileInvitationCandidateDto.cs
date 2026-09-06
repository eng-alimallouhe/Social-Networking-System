namespace SNS.Application.Profiles.Profiles.Contracts;

public record ProfileInvitationCandidateDto(
    Guid Id,
    string FullName,
    string? Specialization,
    string? ProfilePictureUrl,
    bool IsMutualFollow,
    bool FollowsCurrentUser
);
