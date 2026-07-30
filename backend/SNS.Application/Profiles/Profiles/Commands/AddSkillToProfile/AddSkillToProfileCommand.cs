using SNS.Application.Abstractions.Messaging;
using SNS.Domain.Preferences.Enums;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Application.Profiles.Profiles.Commands.AddSkillToProfile;

/// <summary>
/// Represents a command to associate a new skill with the authenticated user's profile.
/// </summary>
/// <param name="SkillId">The unique identifier of the skill to be added.</param>
/// <param name="ProficiencyLevel">The level of proficiency declared for the skill.</param>
public sealed record AddSkillToProfileCommand(
    Guid SkillId, 
    ProficiencyLevel ProficiencyLevel
) : ICommand;

