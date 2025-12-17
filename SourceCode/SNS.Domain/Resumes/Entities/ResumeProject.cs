namespace SNS.Domain.Resumes.Entities;

public class ResumeProject
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid ResumeId { get; set; }
    public Guid ProjectId { get; set; }

    // Navigation
    public Resume Resume { get; set; } = null!;
}
