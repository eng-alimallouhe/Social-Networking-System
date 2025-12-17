using SNS.Domain.Resumes.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Resumes.Entities;

public class Resume
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) To Many(Resumes)
    public Guid OwnerProfileId { get; set; }

    public string PersonalPicture { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    public Template Template { get; set; }
    public string Summary { get; set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Soft Delete
    public bool IsActive { get; set; }

    public ResumeLangauge ResumeLangauge { get; set; }

    // Navigation
    public Profile Owner { get; set; } = null!;
    public ICollection<ResumeEducation> Educations { get; set; } = new List<ResumeEducation>();
    public ICollection<ResumeExperience> Experiences { get; set; } = new List<ResumeExperience>();
    public ICollection<ResumeCertificate> Certificates { get; set; } = new List<ResumeCertificate>();
    public ICollection<ResumeLanguage> Languages { get; set; } = new List<ResumeLanguage>();
}
