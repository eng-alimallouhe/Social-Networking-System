using SNS.Application.Abstractions.Messaging;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Profiles.SocialGraph.Commands.MuteProfile;

/// <summary>
/// Represents a command to mute updates from a target profile for a specified duration.
/// </summary>
/// <param name="TargetProfileId">The unique identifier of the target profile to mute.</param>
/// <param name="Period">The time period duration for muting notifications and updates.</param>
public sealed record MuteProfileCommand(
    Guid TargetProfileId,
    TimePeriod Period
) : ICommand;