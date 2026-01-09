using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
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
    public Guid AuthorId { get; set; }

    // General Properties
    public SolutionStatus Status { get; set; } 

    // Timestamp
    public DateTime CreatedAt { get; set; } 
    public DateTime UpdatedAt { get; set; }
        
    // Soft Delete
    public bool IsActive { get; set; }

    // Navigation Properties
    public Problem Problem { get; set; } = null!;
    public Profile Author { get; set; } = null!;
    public ICollection<SolutionContentBlock> ContentBlocks { get; set; } = new List<SolutionContentBlock>();
    public ICollection<SolutionVote> Votes { get; set; } = new List<SolutionVote>();
    public ICollection<Discussion> Discussions { get; set; } = new List<Discussion>();

    public Solution()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = SolutionStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }
}
