using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Profiles.SocialGraph.Commands.UnMuteProfile;

public sealed record UnMuteProfileCommand(
    Guid TargetProfileId
) : ICommand;