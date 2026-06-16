using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;
using SNS.Domain.Discussions.Shared.Enums;

namespace SNS.Domain.Discussions.Problems.Relations;

public class ProblemVote : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }
    
    //ProblemId and VoterId is Unique (each problem can be voted by the same profile once time)
    // Foreign Key: One(Problem) ? Many(ProblemVotes)
    public Guid ProblemId { get; private set; }

    // Foreign Key: One(Profile) ? Many(ProblemVotes)
    public Guid VoterId { get; private set; }

    // General Properties
    public VoteType Type { get; private set; }

    // Timestamp
    public DateTime CreatedAt { get; private set; }

    // Navigation Properties


    public ProblemVote()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static ProblemVote Create(Guid problemId, Guid voterId, VoteType type)
    {
        return new ProblemVote
        {
            ProblemId = problemId,
            VoterId = voterId,
            Type = type
        };
    }
}
