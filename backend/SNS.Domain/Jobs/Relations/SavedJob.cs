using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Jobs.Entities;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Jobs.Relations;

public class SavedJob : Entity, IHardDeletable
{
    //Primary Key: 
    public Guid Id { get; private set; }

    //Foreign Key: One(Profile) to Many(SavedJob)
    public Guid ProfileId { get; private set; }

    //Foreign Key: One(Job) to Many(SavedJob)
    public Guid JobId { get; private set; }
    public DateTime SavedAt { get; private set; }

    //Navigation Propertie:
    public Job Job { get; private set; } = null!;

    private SavedJob()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        SavedAt = DateTime.UtcNow;
    }

    public static SavedJob Create(Guid profileId, Guid jobId)
    {
        return new SavedJob
        {
            ProfileId = profileId,
            JobId = jobId
        };
    }
}
