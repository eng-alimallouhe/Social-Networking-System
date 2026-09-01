using SNS.Domain.Shared.Helpers;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Resumes.Bridges;
using SNS.Domain.Resumes.Enums;

namespace SNS.Domain.Resumes.Entities;

public class Resume : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) To Many(Resumes)
    public Guid OwnerId { get; private set; }

    public string? PersonalPictureUrl { get; private set; }
    public bool SyncProfilePicture { get; private set; }
    public string Title { get; private set; } = string.Empty;

    public Template Template { get; private set; }
    public string Summary { get; private set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Soft Delete
    public bool IsActive { get; private set; }


    public SupportedLanguage Langauge { get; private set; }

    // Navigation
    public ICollection<ResumeEducation> Educations { get; private set; } = new List<ResumeEducation>();
    public ICollection<ResumeExperience> Experiences { get; private set; } = new List<ResumeExperience>();
    public ICollection<ResumeCertificate> Certificates { get; private set; } = new List<ResumeCertificate>();
    public ICollection<ResumeLanguage> Languages { get; private set; } = new List<ResumeLanguage>();
    public ICollection<ResumeSkill> Skills { get; private set; } = new List<ResumeSkill>();
    public ICollection<ResumeProject> Projects { get; private set; } = new List<ResumeProject>();

    private Resume()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static Resume Create(Guid ownerId, string? personalPictureUrl, bool syncProfilePicture, string title, Template template, string summary, SupportedLanguage langauge)
    {
        return new Resume
        {
            OwnerId = ownerId,
            PersonalPictureUrl = personalPictureUrl,
            SyncProfilePicture = syncProfilePicture,
            Title = title,
            Template = template,
            Summary = summary,
            Langauge = langauge
        };
    }

    public void Update(string? personalPictureUrl, bool syncProfilePicture, string title, Template template, string summary, SupportedLanguage langauge)
    {
        PersonalPictureUrl = personalPictureUrl;
        SyncProfilePicture = syncProfilePicture;
        Title = title;
        Template = template;
        Summary = summary;
        Langauge = langauge;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (IsActive)
        {
            IsActive = false;
        }
    }
}

