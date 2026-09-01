namespace SNS.Application.Profiles.Profiles.Contracts;

/// <summary>
/// Represents summary profile overview details for display in search, suggestion, and list views.
/// </summary>
/// <param name="Id">The unique identifier of the profile.</param>
/// <param name="FullName">The full name of the profile owner.</param>
/// <param name="Specialization">Optional professional specialization.</param>
/// <param name="Bio">Optional biography or summary statement.</param>
/// <param name="ProfilePictureUrl">Optional profile avatar URL or storage key.</param>
/// <param name="FollowersCount">The total count of profile followers.</param>
/// <param name="FollowingCount">The total count of profiles followed.</param>
/// <param name="Skills">The list of skill names associated with the profile.</param>
/// <param name="CreatedAt">The timestamp when the profile was created.</param>
/// <param name="IsFollowedByCurrentUser">Indicates whether the current authenticated profile follows this profile.</param>
/// <param name="IsBlockedByCurrentUser">Indicates whether the current authenticated profile has blocked this profile.</param>
public sealed record ProfileSummaryDto(
    Guid Id,
    string FullName,
    string? Specialization,
    string? Bio,
    string? ProfilePictureUrl,
    int FollowersCount,
    int FollowingCount,
    List<string> Skills,
    DateTime CreatedAt,
    bool IsFollowedByCurrentUser,
    bool IsBlockedByCurrentUser
);
