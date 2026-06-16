using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Discussions.Solutions.Entities;

public class Discussion : Entity, ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Solution) ? Many(Discussions)
    public Guid SolutionId { get; private set; }

    // Foreign Key: One(Discussion) ? Many(Replies) == Optional
    public Guid? ParentDiscussionId { get; private set; }

    // Foreign Key: One(Profile) ? Many(Discussions)
    public Guid AuthorId { get; private set; }

    // General Properties
    public string Text { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string CodeLanguage { get; private set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; private set; } 
    public DateTime UpdatedAt { get; private set; }

    // Soft Delete
    public bool IsActive { get; private set; }

    // Navigation Properties
    public ICollection<Discussion> Replies { get; set; } = new List<Discussion>();

    private Discussion()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static Discussion Create(Guid solutionId, Guid? parentDiscussionId, Guid authorId, string text, string code, string codeLanguage)
    {
        var entity = new Discussion();
        entity.SolutionId = solutionId;
        entity.ParentDiscussionId = parentDiscussionId;
        entity.AuthorId = authorId;
        entity.Text = text;
        entity.Code = code;
        entity.CodeLanguage = codeLanguage;
        return entity;
    }

    public void SoftDelete()
    {
        this.IsActive = false;
    }
}
