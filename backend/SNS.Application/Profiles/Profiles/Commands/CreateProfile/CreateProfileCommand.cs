using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Shared.Contracts.Storage;

namespace SNS.Application.Profiles.Profiles.Commands.CreateProfile;

/// <summary>
/// Represents a command to create an initial profile for the authenticated user.
/// </summary>
/// <param name="FullName">The full display name for the profile.</param>
/// <param name="Specialization">Optional specialization or professional role.</param>
/// <param name="Bio">Optional user biography or summary.</param>
/// <param name="ProfilePicture">Optional uploaded profile picture file.</param>
public sealed record CreateProfileCommand(
    string FullName,
    string? Specialization,
    string? Bio,
    UploadedFile? ProfilePicture
) : ICommand<AuthTokensDto>;