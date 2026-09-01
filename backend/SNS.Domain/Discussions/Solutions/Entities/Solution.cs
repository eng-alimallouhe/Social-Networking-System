using SNS.Domain.Discussions.Solutions.Enums;
using SNS.Domain.Discussions.Solutions.Relations;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Discussions.Solutions.Entities;

public class Solution : Entity, ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Problem) ? Many(Solutions)
    public Guid ProblemId { get; private set; }

    // Foreign Key: One(Profile) ? Many(Solutions)
    public Guid AuthorId { get; private set; }

    // General Properties
    public SolutionStatus Status { get; private set; } 

    // Timestamp
    public DateTime CreatedAt { get; private set; } 
    public DateTime UpdatedAt { get; private set; }
        
    // Soft Delete
    public bool IsActive { get; private set; }

    // Navigation Properties
    public ICollection<SolutionContentBlock> ContentBlocks { get; set; } = new List<SolutionContentBlock>();
    public ICollection<SolutionVote> Votes { get; set; } = new List<SolutionVote>();
    public ICollection<Discussion> Discussions { get; set; } = new List<Discussion>();

    private Solution()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = SolutionStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static Solution Create(Guid problemId, Guid authorId)
    {
        var entity = new Solution();
        entity.ProblemId = problemId;
        entity.AuthorId = authorId;
        return entity;
    }

    public void SoftDelete()
    {
        this.IsActive = false;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(SolutionStatus status)
    {
        this.Status = status;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void Touch()
    {
        this.UpdatedAt = DateTime.UtcNow;
    }
}


