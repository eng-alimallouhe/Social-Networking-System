using SNS.Domain.Abstractions.Common;

namespace SNS.Domain.Resumes.Entities;

public class ResumeExperience : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key
    public Guid ResumeId { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Navigation
    public Resume Resume { get; set; } = null!;
}
