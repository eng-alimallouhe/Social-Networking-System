using SNS.Domain.Shared.Abstractions.IDeletable;

namespace SNS.Domain.Resumes.Entities;

public class ResumeProject : IHardDeletable
{
    // Primary Key (Composite)
    public Guid ResumeId { get; private set; }
    public Guid ProjectId { get; private set; }

    private ResumeProject() { }

    public static ResumeProject Create(Guid resumeId, Guid projectId)
    {
        return new ResumeProject
        {
            ResumeId = resumeId,
            ProjectId = projectId
        };
    }
}
