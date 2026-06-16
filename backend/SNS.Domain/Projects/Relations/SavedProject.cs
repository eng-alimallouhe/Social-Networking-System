using SNS.Domain.Projects.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Projects.Bridges;

public class SavedProject : Entity, IHardDeletable
{
    //Primary Key: 
    public Guid Id { get; private set; }

    //Foreign Key: One(Profile) to Many(SavedProject)
    public Guid ProfileId { get; private set; }

    //Foreign Key: One(Project) to Many(SavedProject)
    public Guid ProjectId { get; private set; }

    public DateTime SavedAt { get; private set; }

    //Navigation Propertie:
    public Project Project { get; private set; } = null!;

    private SavedProject()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        SavedAt = DateTime.UtcNow;
    }

    public static SavedProject Create(Guid profileId, Guid projectId)
    {
        return new SavedProject
        {
            ProfileId = profileId,
            ProjectId = projectId
        };
    }
}
