using SNS.Domain.Preferences.Enums;

namespace SNS.Application.Profiles.Profiles.Contracts;

/// <summary>
/// Represents a data transfer object used to
/// convey details about a specific skill possessed by a user profile.
/// </summary>
/// <param name="Id">Gets the identifier of the skill association.</param>
/// <param name="SkillId">Gets the unique identifier of the skill.</param>
/// <param name="SkillName">Gets the name of the skill.</param>
/// <param name="ProficiencyLevel">Gets the proficiency level of the user in this skill.</param>
public sealed record ProfileSkillDto(
    Guid Id,
    Guid SkillId,
    string SkillName,
    ProficiencyLevel ProficiencyLevel);
