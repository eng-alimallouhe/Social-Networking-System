using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Resumes.Entities;

public class ResumeCertificate : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public Guid ResumeId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;

    public DateTime IssueDate { get; set; }

    // Navigation
    public Resume Resume { get; set; } = null!;

    public ResumeCertificate()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}