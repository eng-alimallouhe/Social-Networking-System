using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;
using SNS.Domain.Discussions.Solutions.Entities;

namespace SNS.Domain.Discussions.Solutions.Relations;

public class SavedSolution : Entity, IHardDeletable
{
    //Primary Key: 
    public Guid Id { get; private set; }

    //Foreign Key: One(Profile) to Many(SavedSolution)
    public Guid ProfileId { get; private set; }

    //Foreign Key: One(Solution) to Many(SavedSolution)
    public Guid SolutionId { get; private  set; }

    public DateTime SavedAt { get; private set; }

    //Navigation Propertie:
    public Solution Solution { get; private set; } = null!;

    public SavedSolution()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        SavedAt = DateTime.UtcNow;
    }

    public static SavedSolution Create(Guid profileId, Guid solutionId)
    {
        return new SavedSolution
        {
            ProfileId = profileId,
            SolutionId = solutionId
        };
    }
}
