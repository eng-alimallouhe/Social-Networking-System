using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Profiles.SocialGraph.Commands.UnfollowProfile;

public sealed record UnfollowProfileCommand(
    Guid TargetProfileId
): ICommand;