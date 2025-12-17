using SNS.Domain.QA.Enums;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Jobs.Entities;


public class JobApplication
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) → Many(JobApplications)
    public Guid ApplicantProfileId { get; set; }

    // Foreign Key: One(Job) → Many(JobApplications)
    public Guid JobId { get; set; }

    // Foreign Key: One(Resume) → Many(JobApplications) == Optional
    public Guid? ResumeId { get; set; }

    public string CoverLetterText { get; set; } = string.Empty;
    public string ResumeFileUrl { get; set; } = string.Empty;
    public ApplicationStatus ApplicationStatus { get; set; }

    //Timestamp
    public DateTime CreatedAt { get; set; }

    
    //Navigation Properties
    public Job Job { get; set; } = null!;
    public Profile ApplicantProfile { get; set; } = null!;

    // Optional Relationship
    public Resume? Resume { get; set; }
}
