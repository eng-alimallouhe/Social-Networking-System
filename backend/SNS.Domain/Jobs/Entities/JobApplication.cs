using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.QA.Enums;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Jobs.Entities;


public class JobApplication : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) → Many(JobApplications)
    public Guid ApplicantId { get; set; }

    // Foreign Key: One(Job) → Many(JobApplications)
    public Guid JobId { get; set; }

    // Foreign Key: One(Resume) → Many(JobApplications) == Optional
    public Guid? ResumeId { get; set; }

    public string CoverLetterText { get; set; } = string.Empty;
    public string? ResumeFileUrl { get; set; }
    public ApplicationStatus Status { get; set; }

    //Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    //Soft Delete
    public bool IsActive { get; set; }


    //Navigation Properties
    public Job Job { get; set; } = null!;
    public Profile Applicant { get; set; } = null!;

    // Optional Relationship
    public Resume? Resume { get; set; }

    public JobApplication()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = ApplicationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}