using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Shared.Enums;

namespace SNS.Domain.Search.Documents;

public class ProblemDocument
{
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) ? Many(Problems)
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorProfilePictureUrl { get; set; } = string.Empty;
    public string AuthorSpecialization { get; set; } = string.Empty;

    // Foreign Key: One(Community) ? Many(Problems) == Optional
    public Guid? CommunityId { get; set; }
    public string? CommunityName { get; set; }
    public string? CommunityLogoUrl { get; set; }

    // General Properties
    public string Title { get; set; } = string.Empty;
    public ProblemStatus Status { get; set; }
    public DifficultyLevel Level { get; set; }

    //This is a the problem blocks as Tags for search preview and filtering
    public List<ProblemBlockDocument> TopTwoContentBlock { get; set; } = new List<ProblemBlockDocument>();

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Soft Delete
    public bool IsActive { get; set; }

    public int UpVotesCount { get; set; }
    public int DownVotesCount { get; set; }

    public int SolutionsCount { get; set; }
    public int ViewsCount { get; set; }
}

public class ProblemBlockDocument
{
    public ProblemBlockType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ExtraInfo { get; set; }
    public int Order { get; set; }
}
