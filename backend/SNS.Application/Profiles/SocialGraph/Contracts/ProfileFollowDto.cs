namespace SNS.Application.Profiles.SocialGraph.Contracts;

/// <summary>
/// Represents data transfer object containing following/follower profile relationship details.
/// </summary>
/// <param name="ProfileId">The unique identifier of the target profile.</param>
/// <param name="FullName">The full name of the profile owner.</param>
/// <param name="Specialization">Optional professional specialization.</param>
/// <param name="ProfilePictureUrl">Optional profile avatar URL.</param>
/// <param name="FollowDate">The timestamp when the follow relationship was established.</param>
public sealed record ProfileFollowDto(
    Guid ProfileId,
    string FullName,
    string? Specialization, 
    string? ProfilePictureUrl,
    DateTime FollowDate);

