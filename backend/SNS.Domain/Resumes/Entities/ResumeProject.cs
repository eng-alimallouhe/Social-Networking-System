using SNS.Domain.Abstractions.Common;

namespace SNS.Domain.Resumes.Entities;

public class ResumeProject : IHardDeletable
{
    // Primary Key (Composite)
    public Guid ResumeId { get; set; }
    public Guid ProjectId { get; set; }
}
