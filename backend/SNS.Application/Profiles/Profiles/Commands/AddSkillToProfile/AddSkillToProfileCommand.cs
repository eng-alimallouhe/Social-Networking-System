using SNS.Application.Abstractions.Messaging;
using SNS.Domain.Preferences.Enums;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Application.Profiles.Profiles.Commands.AddSkillToProfile;

public sealed record AddSkillToProfileCommand(
    Guid SkillId, 
    ProficiencyLevel ProficiencyLevel
) : ICommand;
