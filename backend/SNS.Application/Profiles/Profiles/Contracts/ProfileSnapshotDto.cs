namespace SNS.Application.Profiles.Profiles.Contracts;

/// <summary>
/// Represents a lightweight snapshot of profile information for embedding in posts, comments, reactions, and other features.
/// </summary>
/// <param name="Id">The unique identifier of the profile.</param>
/// <param name="FullName">The display / full name of the user profile.</param>
/// <param name="Specialization">Optional professional specialization.</param>
/// <param name="ProfilePictureUrl">Optional resolved temporary profile picture URL.</param>
public sealed record ProfileSnapshotDto(
    Guid Id,
    string FullName,
    string? Specialization,
    string? ProfilePictureUrl
);
