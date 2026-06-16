using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Projects.Entities;

namespace SNS.Domain.Projects.Bridges;

public class ProjectTag : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Keys
    public Guid ProjectId { get; private set; }
    public Guid TagId { get; private set; }

    // Navigation



    private ProjectTag()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static ProjectTag Create(Guid projectId, Guid tagId)
    {
        return new ProjectTag
        {
            ProjectId = projectId,
            TagId = tagId
        };
    }
}
