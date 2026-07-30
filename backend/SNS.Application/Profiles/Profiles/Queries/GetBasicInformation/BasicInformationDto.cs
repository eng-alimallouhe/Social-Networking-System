namespace SNS.Application.Profiles.Profiles.Queries.GetBasicInformation;

/// <summary>
/// Represents data transfer object containing basic profile summary information.
/// </summary>
/// <param name="FullName">The full display name of the profile owner.</param>
/// <param name="Bio">Optional biography text.</param>
/// <param name="ProfilePictureUrl">Optional profile avatar URL.</param>
/// <param name="Specialization">Optional primary specialization.</param>
/// <param name="Reputation">The total reputation score earned by the profile.</param>
public sealed record BasicInformationDto(
    string FullName,
    string? Bio,
    string? ProfilePictureUrl,
    string? Specialization,
    int Reputation
);

