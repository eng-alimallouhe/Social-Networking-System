using SNS.Application.Abstractions.Messaging;
using SNS.Application.Shared.Contracts.Storage;

namespace SNS.Application.Profiles.Profiles.Commands.UpdateProfilePicture;

public sealed record UpdateProfilePictureCommand(
    UploadedFile ProfilePictureFile) : ICommand;
