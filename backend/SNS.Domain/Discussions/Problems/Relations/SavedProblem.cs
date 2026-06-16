using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;
using SNS.Domain.Discussions.Problems.Entities;

namespace SNS.Domain.Discussions.Problems.Relations;

public class SavedProblem : Entity, IHardDeletable
{
    //Primary Key: 
    public Guid Id { get; private set; }

    //the ProfileId with the ProblemId should be unique together, to prevent the same user from saving the same problem multiple times.
    //Foreign Key: One(Profile) to Many(SavedProblem)
    public Guid ProfileId { get; private set; } 
    //Foreign Key: One(Problem) to Many(SavedProblem)
    public Guid ProblemId { get; private set; }

    public DateTime SavedAt { get; private set; }

    //Navigation Propertie:
    public Problem Problem { get; private set; } = null!;

    public SavedProblem()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        SavedAt = DateTime.UtcNow;
    }

    public static SavedProblem Create(Guid profileId, Guid problemId)
    {
        return new SavedProblem
        {
            ProfileId = profileId,
            ProblemId = problemId
        };
    }
}
