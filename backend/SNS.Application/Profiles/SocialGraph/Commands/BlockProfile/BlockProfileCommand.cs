using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Profiles.SocialGraph.Commands.BlockProfile;

/// <summary>
/// Represents a command to block another profile in the social graph.
/// </summary>
/// <param name="TargetProfileId">The unique identifier of the target profile to be blocked.</param>
public sealed record BlockProfileCommand(
    Guid TargetProfileId
) : ICommand;

