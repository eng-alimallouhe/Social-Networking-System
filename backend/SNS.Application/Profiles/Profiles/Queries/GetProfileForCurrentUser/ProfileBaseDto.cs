namespace SNS.Application.Profiles.Profiles.Contracts;

/// <summary>
/// Represents basic profile summary data transfer object.
/// </summary>
/// <param name="Id">The unique identifier of the profile.</param>
/// <param name="FullName">The full display name of the user profile.</param>
/// <param name="Specialization">The user's primary professional specialization.</param>
/// <param name="ProfilePictureUrl">The URL of the profile avatar image.</param>
/// <param name="Reputation">The total reputation points earned by the user.</param>
public sealed record ProfileBaseDto(
    Guid Id,
    string FullName,
    string Specialization,
    string ProfilePictureUrl,
    int Reputation);

