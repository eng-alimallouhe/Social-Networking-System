using SNS.Domain.Resumes.Enums;

namespace SNS.Domain.Resumes.Entities;

public class ResumeLanguage
{
    // Primary Key
    public Guid Id { get; set; }

    // FK
    public Guid ResumeId { get; set; }

    public LanguageLevel Level { get; set; }
    public Language Language { get; set; }

    // Navigation
    public Resume Resume { get; set; } = null!;
}
