using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.QA.Entities;
using SNS.Domain.QA.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.QA.Bridges;

public class SolutionVote : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Solution) → Many(SolutionVotes)
    public Guid SolutionId { get; set; }

    // Foreign Key: One(Profile) → Many(SolutionVotes)
    public Guid VoterId { get; set; }

    // General Properties
    public VoteType Type { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public Solution Solution { get; set; } = null!;
    public Profile Voter { get; set; } = null!;

    public SolutionVote()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }
}