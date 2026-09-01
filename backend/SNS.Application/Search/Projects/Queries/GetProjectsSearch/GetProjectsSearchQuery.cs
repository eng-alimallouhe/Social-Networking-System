using SNS.Application.Projects.Contracts;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Projects.Enums;

namespace SNS.Application.Search.Projects.Queries.GetProjectsSearch;

/// <summary>
/// Represents a search query to search project documents in the search index using specified filter criteria.
/// </summary>
/// <param name="SearchTerm">Optional keyword or phrase to search within project title and description.</param>
/// <param name="Status">Optional project status filter.</param>
/// <param name="MinCreatedAt">Optional minimum project creation date filter.</param>
/// <param name="MaxCreatedAt">Optional maximum project creation date filter.</param>
/// <param name="RequiredSkills">Optional list of required skills to filter projects.</param>
/// <param name="MinContributors">Optional minimum contributor count filter.</param>
/// <param name="MaxContributors">Optional maximum contributor count filter.</param>
/// <param name="MinRate">Optional minimum rate filter.</param>
/// <param name="Page">The page index for pagination (1-based).</param>
/// <param name="PageSize">The maximum number of project records to return per page.</param>
public sealed record GetProjectsSearchQuery(
    string? SearchTerm = null,
    ProjectStatus? Status = null,
    DateTime? MinCreatedAt = null,
    DateTime? MaxCreatedAt = null,
    List<string>? RequiredSkills = null,
    int? MinContributors = null,
    int? MaxContributors = null,
    decimal? MinRate = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<SearchResult<ProjectOverviewDto>>;
