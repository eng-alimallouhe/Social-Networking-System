using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Profiles.SocialGraph.Commands.FollowProfile;

public sealed record FollowProfileCommand(
    Guid TargetProfileId
) : ICommand;