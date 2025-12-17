using SNS.Domain.Resumes.Enums;

namespace SNS.Domain.Resumes.Entities;

public class ResumeSkill
{
    // Primary Key
    public Guid Id { get; set; }

    public Guid ResumeId { get; set; }

    public string SkillName { get; set; } = string.Empty;
    public ResumeSkillLevel Level { get; set; }

    // Navigation
    public Resume Resume { get; set; } = null!;
}
