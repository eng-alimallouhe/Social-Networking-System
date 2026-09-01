using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;
using SNS.Domain.Resumes.Enums;

namespace SNS.Domain.Resumes.Entities;

public class ResumeLanguage : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // FK
    public Guid ResumeId { get; private set; }

    public LanguageLevel Level { get; private set; }
    public Language Language { get; private set; }

    private ResumeLanguage()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static ResumeLanguage Create(Guid resumeId, Language language, LanguageLevel level)
    {
        return new ResumeLanguage
        {
            ResumeId = resumeId,
            Language = language,
            Level = level
        };
    }

    public void Update(Language language, LanguageLevel level)
    {
        Language = language;
        Level = level;
    }
}
