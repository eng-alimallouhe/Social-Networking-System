using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Resumes.Entities;

public class ResumeCertificate : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    public Guid ResumeId { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string Issuer { get; private set; } = string.Empty;

    public DateTime IssueDate { get; private set; }

    private ResumeCertificate()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static ResumeCertificate Create(Guid resumeId, string title, string issuer, DateTime issueDate)
    {
        return new ResumeCertificate
        {
            ResumeId = resumeId,
            Title = title,
            Issuer = issuer,
            IssueDate = issueDate
        };
    }

    public void Update(string title, string issuer, DateTime issueDate)
    {
        Title = title;
        Issuer = issuer;
        IssueDate = issueDate;
    }
}
