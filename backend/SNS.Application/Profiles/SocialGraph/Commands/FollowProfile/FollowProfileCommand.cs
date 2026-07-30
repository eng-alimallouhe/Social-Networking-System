using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Profiles.SocialGraph.Commands.FollowProfile;

/// <summary>
/// Represents a command to follow another profile in the social graph.
/// </summary>
/// <param name="TargetProfileId">The unique identifier of the target profile to follow.</param>
public sealed record FollowProfileCommand(
    Guid TargetProfileId
) : ICommand;