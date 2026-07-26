using SNS.Application.Abstractions.Messaging;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Profiles.SocialGraph.Commands.MuteProfile;

public sealed record MuteProfileCommand(
    Guid TargetProfileId,
    TimePeriod Period
) : ICommand;