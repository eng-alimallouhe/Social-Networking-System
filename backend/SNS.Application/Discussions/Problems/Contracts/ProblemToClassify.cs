using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Shared.Enums;

namespace SNS.Application.Discussions.Problems.Contracts;

/// <summary>
/// Represents problem snapshot data for topic classification and search indexing.
/// </summary>
public sealed class ProblemToClassify
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorSpecialization { get; set; } = string.Empty;
    public string? AuthorProfilePictureObjectKey { get; set; } = string.Empty;
    public Guid? CommunityId { get; set; }
    public string Title { get; set; } = string.Empty;
    public ProblemStatus Status { get; set; }
    public DifficultyLevel Level { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ProblemContentBlockSnapshot> ContentBlocks { get; set; } = new();
    public List<string> Topics { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Represents a snapshot of a problem content block.
/// </summary>
public sealed class ProblemContentBlockSnapshot
{
    public ProblemBlockType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ExtraInfo { get; set; }
    public int Order { get; set; }
}
