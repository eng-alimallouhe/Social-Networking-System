namespace SNS.Application.Projects.Contracts;

public record ProjectParticipantDetailsDto(
    Guid ProfileId,
    string? ProfileImageUrl,
    string DisplayName,
    string? Specialization,
    int FollowersCount,
    int FollowingCount,
    bool IsFollowedByCurrentUser,
    string Role
);
