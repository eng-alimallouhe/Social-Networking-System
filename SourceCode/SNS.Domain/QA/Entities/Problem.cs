using SNS.Domain.Communities.Entities;
using SNS.Domain.QA.Bridges;
using SNS.Domain.QA.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.QA.Entities;

public class Problem
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) → Many(Problems)
    public Guid AuthorProfileId { get; set; }

    // Foreign Key: One(Community) → Many(Problems) == Optional
    public Guid? CommunityId { get; set; }

    // General Properties
    public string Title { get; set; } = string.Empty;
    public ProblemStatus Status { get; set; } = ProblemStatus.Open;
    public string ReadmeContent { get; set; } = string.Empty;
    public DifficultyLevel Level { get; set; } = DifficultyLevel.Medium;

    // Timestamp
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Soft Delete
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Profile AuthorProfile { get; set; } = null!;
    public Community? Community { get; set; }
    public ICollection<ProblemContentBlock> ContentBlocks { get; set; } = new List<ProblemContentBlock>();
    public ICollection<ProblemVote> Votes { get; set; } = new List<ProblemVote>();
    public ICollection<ProblemTag> Tags { get; set; } = new List<ProblemTag>();
    public ICollection<ProblemTopic> Topics { get; set; } = new List<ProblemTopic>();
    public ICollection<Solution> Solutions { get; set; } = new List<Solution>();
    public ICollection<ProblemView> Views { get; set; } = new List<ProblemView>();
}