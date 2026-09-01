using SNS.Application.Resumes.Certificates.Contracts;
using SNS.Application.Resumes.Educations.Contracts;
using SNS.Application.Resumes.Experiences.Contracts;
using SNS.Application.Resumes.Languages.Contracts;
using SNS.Application.Resumes.Projects.Contracts;
using SNS.Application.Resumes.Skills.Contracts;
using SNS.Domain.Resumes.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Resumes.Resumes.Contracts;

/// <summary>
/// Represents the comprehensive detailed view of a resume including all child sections.
/// </summary>
/// <param name="Id">The unique identifier of the resume.</param>
/// <param name="OwnerId">The profile identifier of the resume owner.</param>
/// <param name="PersonalPictureUrl">The temporary presigned URL for the personal picture.</param>
/// <param name="SyncProfilePicture">Indicates whether the resume synchronizes its picture with the profile avatar.</param>
/// <param name="Title">The title or professional designation on the resume.</param>
/// <param name="Template">The visual layout template of the resume.</param>
/// <param name="Summary">The professional summary or executive statement.</param>
/// <param name="Language">The supported language of the resume.</param>
/// <param name="CreatedAt">The timestamp when the resume was created.</param>
/// <param name="UpdatedAt">The timestamp when the resume was last updated.</param>
/// <param name="Educations">The collection of educational history entries.</param>
/// <param name="Experiences">The collection of work experience entries.</param>
/// <param name="Certificates">The collection of professional certifications.</param>
/// <param name="Languages">The collection of language proficiencies.</param>
/// <param name="Skills">The collection of technical and professional skills.</param>
/// <param name="Projects">The collection of linked showcase projects.</param>
public sealed record ResumeDetailsDto(
    Guid Id,
    Guid OwnerId,
    string? PersonalPictureUrl,
    bool SyncProfilePicture,
    string Title,
    Template Template,
    string Summary,
    SupportedLanguage Language,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<ResumeEducationDto> Educations,
    List<ResumeExperienceDto> Experiences,
    List<ResumeCertificateDto> Certificates,
    List<ResumeLanguageDto> Languages,
    List<ResumeSkillDto> Skills,
    List<ResumeProjectDto> Projects
);
