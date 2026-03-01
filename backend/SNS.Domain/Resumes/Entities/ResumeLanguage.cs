using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Resumes.Enums;

namespace SNS.Domain.Resumes.Entities;

public class ResumeLanguage : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // FK
    public Guid ResumeId { get; set; }

    public LanguageLevel Level { get; set; }
    public Language Language { get; set; }

    public ResumeLanguage()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}
