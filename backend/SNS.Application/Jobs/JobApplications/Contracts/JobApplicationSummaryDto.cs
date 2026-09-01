using SNS.Domain.QA.Enums;

namespace SNS.Application.Jobs.JobApplications.Contracts;

public sealed record JobApplicationSummaryDto(
    Guid Id,
    Guid JobId,
    string JobTitle,
    string CompanyName,
    Guid ApplicantId,
    string ApplicantFullName,
    string? ApplicantAvatarUrl,
    string? ApplicantSpecialization,
    Guid? ResumeId,
    string? ResumeFileUrl,
    ApplicationStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
