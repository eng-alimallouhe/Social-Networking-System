using SNS.Domain.Abstractions.Common;
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
    public Guid VoterProfileId { get; set; }

    // General Properties
    public VoteType VoteType { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Solution Solution { get; set; } = null!;
    public Profile VoterProfile { get; set; } = null!;
}
