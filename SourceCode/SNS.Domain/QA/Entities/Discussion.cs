using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.QA.Entities;

public class Discussion : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Solution) → Many(Discussions)
    public Guid SolutionId { get; set; }

    // Foreign Key: One(Discussion) → Many(Replies) == Optional
    public Guid? ParentDiscussionId { get; set; }

    // Foreign Key: One(Profile) → Many(Discussions)
    public Guid AuthorProfileId { get; set; }

    // General Properties
    public string Text { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string CodeLanguage { get; set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; set; } 
    public DateTime UpdatedAt { get; set; }

    // Soft Delete
    public bool IsActive { get; set; }

    // Navigation Properties
    public Solution Solution { get; set; } = null!;
    public Discussion? ParentDiscussion { get; set; }
    public Profile AuthorProfile { get; set; } = null!;
    public ICollection<Discussion> Replies { get; set; } = new List<Discussion>();

    public Discussion()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }
}