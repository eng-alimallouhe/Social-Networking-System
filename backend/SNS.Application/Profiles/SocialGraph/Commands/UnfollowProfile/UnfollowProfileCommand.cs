using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Profiles.SocialGraph.Commands.UnfollowProfile;

/// <summary>
/// Represents a command to unfollow a previously followed profile in the social graph.
/// </summary>
/// <param name="TargetProfileId">The unique identifier of the target profile to unfollow.</param>
public sealed record UnfollowProfileCommand(
    Guid TargetProfileId
): ICommand;