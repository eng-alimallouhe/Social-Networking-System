using SNS.Domain.Resumes.Enums;

namespace SNS.Application.Resumes.Languages.Contracts;

/// <summary>
/// Represents a language proficiency record within a resume.
/// </summary>
/// <param name="Id">The unique identifier of the language record.</param>
/// <param name="ResumeId">The identifier of the parent resume.</param>
/// <param name="Language">The spoken or written language.</param>
/// <param name="Level">The proficiency level in the language.</param>
public sealed record ResumeLanguageDto(
    Guid Id,
    Guid ResumeId,
    Language Language,
    LanguageLevel Level
);
