using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Communities.Entities;
using SNS.Domain.QA.Bridges;
using SNS.Domain.QA.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.QA.Entities;

public class Problem : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) → Many(Problems)
    public Guid AuthorId { get; set; }

    // Foreign Key: One(Community) → Many(Problems) == Optional
    public Guid? CommunityId { get; set; }

    // General Properties
    public string Title { get; set; } = string.Empty;
    public ProblemStatus Status { get; set; }
    public string ReadmeContent { get; set; } = string.Empty;
    public DifficultyLevel Level { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Soft Delete
    public bool IsActive { get; set; }

    // Navigation Properties
    public Profile Author { get; set; } = null!;
    
    public Community? Community { get; set; }
    
    public ICollection<ProblemContentBlock> ContentBlocks { get; set; } 
        = new List<ProblemContentBlock>();
    
    public ICollection<ProblemVote> Votes { get; set; } 
        = new List<ProblemVote>();
    
    public ICollection<ProblemTag> Tags { get; set; } 
        = new List<ProblemTag>();
    
    public ICollection<ProblemTopic> Topics { get; set; } 
        = new List<ProblemTopic>();
    
    public ICollection<Solution> Solutions { get; set; } 
        = new List<Solution>();
    
    public ICollection<ProblemView> Views { get; set; } 
        = new List<ProblemView>();

    public Problem()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = ProblemStatus.Open;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }
}