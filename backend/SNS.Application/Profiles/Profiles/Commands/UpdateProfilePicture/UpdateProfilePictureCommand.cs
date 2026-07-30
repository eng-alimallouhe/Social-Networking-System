using SNS.Application.Abstractions.Messaging;
using SNS.Application.Shared.Contracts.Storage;

namespace SNS.Application.Profiles.Profiles.Commands.UpdateProfilePicture;

/// <summary>
/// Represents a command to update or replace the profile picture of the authenticated user.
/// </summary>
/// <param name="ProfilePictureFile">The uploaded profile picture file containing the file stream and content details.</param>
public sealed record UpdateProfilePictureCommand(
    UploadedFile ProfilePictureFile) : ICommand;

