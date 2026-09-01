using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Domain.Discussions.Solutions.Entities;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Discussions.Problems.Entities;

public class Problem : Entity, ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) ? Many(Problems)
    public Guid AuthorId { get; private set; }

    // Foreign Key: One(Community) ? Many(Problems) == Optional
    public Guid? CommunityId { get; private set; }

    // General Properties
    public string Title { get; private set; } = string.Empty;
    public ProblemStatus Status { get; private set; }
    public DifficultyLevel Level { get; private set; }

    // Timestamp
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    // Soft Delete
    public bool IsActive { get; private set; }

    // Navigation Properties
    public Community? Community { get; set; }

    public Profile Author { get; set; } = null!;

    public ICollection<ProblemContentBlock> ContentBlocks { get; set; } 
        = new List<ProblemContentBlock>();
    

    public ICollection<ProblemTag> ProblemTags { get; set; } 
        = new List<ProblemTag>();
    

    public ICollection<ProblemTopic> ProblemTopics { get; set; } 
        = new List<ProblemTopic>();
    
    public ICollection<ProblemVote> Votes { get; set; } 
        = new List<ProblemVote>();
    
    public ICollection<Solution> Solutions { get; set; } 
        = new List<Solution>();
    
    public ICollection<ProblemView> Views { get; set; } 
        = new List<ProblemView>();

    private Problem()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = ProblemStatus.Open;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static Problem Create(Guid authorId, Guid? communityId, string title, DifficultyLevel level)
    {
        var entity = new Problem();
        entity.AuthorId = authorId;
        entity.CommunityId = communityId;
        entity.Title = title;
        entity.Level = level;
        return entity;
    }

    public void SoftDelete()
    {
        this.IsActive = false;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string title, DifficultyLevel level, Guid? communityId)
    {
        this.Title = title;
        this.Level = level;
        this.CommunityId = communityId;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(ProblemStatus status)
    {
        this.Status = status;
        this.UpdatedAt = DateTime.UtcNow;
    }
}
