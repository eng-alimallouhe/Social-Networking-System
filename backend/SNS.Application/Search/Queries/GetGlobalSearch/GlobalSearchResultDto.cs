using SNS.Domain.Search.Documents;

namespace SNS.Application.Search.Queries.GetGlobalSearch;

/// <summary>
/// Represents response DTO for global search containing matched top results grouped across multiple entity categories.
/// </summary>
public sealed record GlobalSearchResultDto
{
    /// <summary>
    /// Gets matched user profile documents.
    /// </summary>
    public List<ProfileDocument> Profiles { get; init; } = new();

    /// <summary>
    /// Gets matched project documents.
    /// </summary>
    public List<ProjectDocument> Projects { get; init; } = new();

    /// <summary>
    /// Gets matched community documents.
    /// </summary>
    public List<CommunityDocument> Communities { get; init; } = new();

    /// <summary>
    /// Gets matched job posting documents.
    /// </summary>
    public List<JobsDocument> Jobs { get; init; } = new();

    /// <summary>
    /// Gets matched discussion problem documents.
    /// </summary>
    public List<ProblemDocument> Problems { get; init; } = new();
}

