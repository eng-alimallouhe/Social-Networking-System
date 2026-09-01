using SNS.Application.Jobs.Jobs.Contracts;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.QA.Enums;

namespace SNS.Application.Jobs.JobApplications.Contracts;

public sealed record JobApplicationDetailsDto(
    Guid Id,
    Guid JobId,
    JobSnapshotDto Job,
    Guid ApplicantId,
    ProfileSnapshotDto Applicant,
    Guid? ResumeId,
    string CoverLetterText,
    string? ResumeFileUrl,
    ApplicationStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsActive
);
