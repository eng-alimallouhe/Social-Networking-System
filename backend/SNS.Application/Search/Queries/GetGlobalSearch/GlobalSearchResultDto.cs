using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Discussions.Problems.Problems.Contracts;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Projects.Contracts;
using SNS.Application.Search.Jobs.Contracts;

namespace SNS.Application.Search.Queries.GetGlobalSearch;

/// <summary>
/// Represents response DTO for global search containing matched top results grouped across multiple entity categories.
/// </summary>
public sealed record GlobalSearchResultDto
{
    /// <summary>
    /// Gets matched user profile summaries.
    /// </summary>
    public List<ProfileSummaryDto> Profiles { get; init; } = new();

    /// <summary>
    /// Gets matched project overviews.
    /// </summary>
    public List<ProjectOverviewDto> Projects { get; init; } = new();

    /// <summary>
    /// Gets matched community summaries.
    /// </summary>
    public List<CommunitySummaryDto> Communities { get; init; } = new();

    /// <summary>
    /// Gets matched job summaries.
    /// </summary>
    public List<JobSummaryDto> Jobs { get; init; } = new();

    /// <summary>
    /// Gets matched discussion problem summaries.
    /// </summary>
    public List<ProblemSummaryDto> Problems { get; init; } = new();

    /// <summary>
    /// Gets matched post overviews.
    /// </summary>
    public List<PostOverviewDto> Posts { get; init; } = new();
}
