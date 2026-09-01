namespace SNS.Application.Resumes.Educations.Contracts;

/// <summary>
/// Represents educational qualification details within a resume.
/// </summary>
/// <param name="Id">The unique identifier of the education record.</param>
/// <param name="ResumeId">The identifier of the parent resume.</param>
/// <param name="UniversityName">The university or educational institution name.</param>
/// <param name="FacultyName">The faculty or college name.</param>
/// <param name="Degree">The degree obtained or pursued.</param>
/// <param name="FieldOfStudy">The major or field of study.</param>
/// <param name="StartDate">The start date of the education.</param>
/// <param name="EndDate">The optional graduation or end date.</param>
/// <param name="GPA">The optional grade point average.</param>
public sealed record ResumeEducationDto(
    Guid Id,
    Guid ResumeId,
    string UniversityName,
    string FacultyName,
    string Degree,
    string FieldOfStudy,
    DateTime StartDate,
    DateTime? EndDate,
    double? GPA
);
