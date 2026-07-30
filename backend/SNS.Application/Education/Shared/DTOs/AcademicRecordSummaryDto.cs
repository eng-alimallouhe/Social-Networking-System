namespace SNS.Application.Education.Shared.DTOs;

/// <summary>
/// Represents data transfer object summarizing an academic education background record.
/// </summary>
/// <param name="UniversityName">The name of the university or academic institution.</param>
/// <param name="FieldOfStudy">Optional degree specialization or field of study.</param>
public sealed record AcademicRecordSummaryDto(
    string UniversityName,
    string? FieldOfStudy);

