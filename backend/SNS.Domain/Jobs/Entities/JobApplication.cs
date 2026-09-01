using SNS.Domain.QA.Enums;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Jobs.Entities;

public class JobApplication : Entity, ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) → Many(JobApplications)
    public Guid ApplicantId { get; private set; }

    // Foreign Key: One(Job) → Many(JobApplications)
    public Guid JobId { get; private set; }

    // Foreign Key: One(Resume) → Many(JobApplications) == Optional
    public Guid? ResumeId { get; private set; }

    public string CoverLetterText { get; private set; } = string.Empty;
    public string? ResumeFileUrl { get; private set; }
    public ApplicationStatus Status { get; private set; }

    // Timestamp
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Soft Delete
    public bool IsActive { get; private set; }

    private JobApplication()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = ApplicationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static JobApplication Create(
        Guid applicantId,
        Guid jobId,
        Guid? resumeId,
        string coverLetterText,
        string? resumeFileUrl = null,
        ApplicationStatus status = ApplicationStatus.Pending)
    {
        return new JobApplication
        {
            ApplicantId = applicantId,
            JobId = jobId,
            ResumeId = resumeId,
            CoverLetterText = coverLetterText,
            ResumeFileUrl = resumeFileUrl,
            Status = status,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void ChangeStatus(ApplicationStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Withdraw()
    {
        Status = ApplicationStatus.Withdrawn;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (IsActive)
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
