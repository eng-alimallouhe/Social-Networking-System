using SNS.Domain.Abstractions.Common;
using SNS.Domain.QA.Bridges;
using SNS.Domain.QA.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.QA.Entities;

public class Solution : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Problem) → Many(Solutions)
    public Guid ProblemId { get; set; }

    // Foreign Key: One(Profile) → Many(Solutions)
    public Guid AuthorProfileId { get; set; }

    // General Properties
    public SolutionStatus Status { get; set; } = SolutionStatus.Pending;

    // Timestamp
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
        
    // Soft Delete
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Problem Problem { get; set; } = null!;
    public Profile AuthorProfile { get; set; } = null!;
    public ICollection<SolutionContentBlock> ContentBlocks { get; set; } = new List<SolutionContentBlock>();
    public ICollection<SolutionVote> Votes { get; set; } = new List<SolutionVote>();
    public ICollection<Discussion> Discussions { get; set; } = new List<Discussion>();
}
