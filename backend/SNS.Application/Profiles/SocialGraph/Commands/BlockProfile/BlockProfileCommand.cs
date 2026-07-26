using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Profiles.SocialGraph.Commands.BlockProfile;

public sealed record BlockProfileCommand(
    Guid TargetProfileId
) : ICommand;
