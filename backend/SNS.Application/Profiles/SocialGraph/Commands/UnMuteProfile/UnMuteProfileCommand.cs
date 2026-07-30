using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Profiles.SocialGraph.Commands.UnMuteProfile;

/// <summary>
/// Represents a command to unmute updates from a previously muted profile in the social graph.
/// </summary>
/// <param name="TargetProfileId">The unique identifier of the target profile to unmute.</param>
public sealed record UnMuteProfileCommand(
    Guid TargetProfileId
) : ICommand;