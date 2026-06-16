using SNS.Domain.Discussions.Shared.Enums;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Discussions.Solutions.Relations;

public class SolutionVote : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }


    //SolutionId and VoterId is Unique (each solution can be voted by the same profile once time)
    // Foreign Key: One(Solution) ? Many(SolutionVotes)
    public Guid SolutionId { get; private set; }

    // Foreign Key: One(Profile) ? Many(SolutionVotes)
    public Guid VoterId { get; private set; }

    // General Properties
    public VoteType Type { get; private set; }

    // Timestamp
    public DateTime CreatedAt { get; private set; }

    // Navigation Properties



    public SolutionVote()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static SolutionVote Create(Guid profileId, Guid solutionId, VoteType type)
    {
        return new SolutionVote
        {
            VoterId = profileId,
            SolutionId = solutionId,
            Type = type
        };
    }

    public void ChangeVote(VoteType type)
    {
        Type = type;
    }
}
