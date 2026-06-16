using SNS.Domain.Shared.Helpers;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.QA.Enums;

namespace SNS.Domain.Jobs.Entities;


public class JobApplication : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) ? Many(JobApplications)
    public Guid ApplicantId { get; private set; }

    // Foreign Key: One(Job) ? Many(JobApplications)
    public Guid JobId { get; private set; }

    // Foreign Key: One(Resume) ? Many(JobApplications) == Optional
    public Guid? ResumeId { get; private set; }

    public string CoverLetterText { get; private set; } = string.Empty;
    public string? ResumeFileUrl { get; private set; }
    public ApplicationStatus Status { get; private set; }

    //Timestamp
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    //Soft Delete
    public bool IsActive { get; private set; }


    private JobApplication()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = ApplicationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static JobApplication Create(Guid profileId, Guid jobId, Guid? resumeId,
     string coverLetterText, string? resumeFileUrl, ApplicationStatus status)
    {
        return new JobApplication
        {
            ApplicantId = profileId,
            JobId = jobId,
            ResumeId = resumeId,
            CoverLetterText = coverLetterText,
            ResumeFileUrl = resumeFileUrl,
            Status = status
        };
    }

    public void SoftDelete()
    {
        if (IsActive)
        {
            IsActive = false;
        }
    }
}

