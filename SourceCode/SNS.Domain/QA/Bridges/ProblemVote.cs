using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.QA.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.QA.Entities;

public class ProblemVote : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Problem) → Many(ProblemVotes)
    public Guid ProblemId { get; set; }

    // Foreign Key: One(Profile) → Many(ProblemVotes)
    public Guid VoterProfileId { get; set; }

    // General Properties
    public VoteType Type { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public Problem Problem { get; set; } = null!;
    public Profile VoterProfile { get; set; } = null!;

    public ProblemVote()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }
}