using SNS.Application.Projects.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Projects.Queries.GetProjectFeed;

/// <summary>
/// Represents a query to retrieve a personalized feed of projects for the authenticated user.
/// </summary>
/// <param name="CurrentPage">The current page index for feed pagination (1-based).</param>
/// <param name="PageSize">The maximum number of projects to retrieve per page.</param>
public sealed record GetProjectFeedQuery(
    int CurrentPage,
    int PageSize
) : IQuery<List<ProjectOverviewDto>>;
