using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Shared.Enums;

namespace SNS.Domain.Search.Documents;

public class ProblemDocument
{
    public Guid Id { get; set; }

    // General Properties
    public string Title { get; set; } = string.Empty;
    public ProblemStatus Status { get; set; }
    public DifficultyLevel Level { get; set; }

    //This is a the problem blocks as Tags for search preview and filtering
    public List<ProblemBlockDocument> ContentBlocks { get; set; } = new List<ProblemBlockDocument>();

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<string> Topics { get; set; } = new List<string>();
    public List<string> Tags { get; set; } = new List<string>();
}

public class ProblemBlockDocument
{
    public ProblemBlockType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ExtraInfo { get; set; }
    public int Order { get; set; }
}
