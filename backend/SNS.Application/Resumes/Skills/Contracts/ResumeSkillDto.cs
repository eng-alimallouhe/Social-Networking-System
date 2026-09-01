using SNS.Domain.Resumes.Enums;

namespace SNS.Application.Resumes.Skills.Contracts;

/// <summary>
/// Represents a technical or professional skill entry within a resume.
/// </summary>
/// <param name="Id">The unique identifier of the skill entry.</param>
/// <param name="ResumeId">The identifier of the parent resume.</param>
/// <param name="SkillName">The name of the skill.</param>
/// <param name="Level">The competency level of the skill.</param>
public sealed record ResumeSkillDto(
    Guid Id,
    Guid ResumeId,
    string SkillName,
    ResumeSkillLevel Level
);
