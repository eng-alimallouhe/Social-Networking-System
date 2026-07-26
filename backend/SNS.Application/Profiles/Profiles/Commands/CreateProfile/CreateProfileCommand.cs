using SNS.Application.Abstractions.Messaging;
using SNS.Application.Shared.Contracts.Storage;

namespace SNS.Application.Profiles.Profiles.Commands.CreateProfile;

public sealed record CreateProfileCommand(
    string FullName,
    string? Specialization,
    string? Bio,
    UploadedFile? ProfilePicture
) : ICommand;