namespace SNS.Application.Resumes.Experiences.Contracts;

/// <summary>
/// Represents professional work experience details within a resume.
/// </summary>
/// <param name="Id">The unique identifier of the experience record.</param>
/// <param name="ResumeId">The identifier of the parent resume.</param>
/// <param name="CompanyName">The name of the company or organization.</param>
/// <param name="Position">The job title or position held.</param>
/// <param name="Description">The summary of responsibilities and achievements.</param>
/// <param name="StartDate">The start date of the employment.</param>
/// <param name="EndDate">The optional end date of the employment.</param>
public sealed record ResumeExperienceDto(
    Guid Id,
    Guid ResumeId,
    string CompanyName,
    string Position,
    string Description,
    DateTime StartDate,
    DateTime? EndDate
);
