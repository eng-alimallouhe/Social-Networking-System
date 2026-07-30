namespace SNS.Application.Profiles.Profiles.Contracts;

/// <summary>
/// Represents data transfer object containing details of a profile view history event.
/// </summary>
/// <param name="ProfileId">The unique identifier of the viewed profile.</param>
/// <param name="FullName">The full name of the profile owner.</param>
/// <param name="ProfilePictureUrl">Optional profile picture URL.</param>
/// <param name="Specialization">Optional professional specialization.</param>
/// <param name="ViewedAt">The timestamp when the profile view occurred.</param>
public sealed record ProfileViewDto(
    Guid ProfileId,
    string FullName,
    string? ProfilePictureUrl,
    string? Specialization,
    DateTime ViewedAt
);

