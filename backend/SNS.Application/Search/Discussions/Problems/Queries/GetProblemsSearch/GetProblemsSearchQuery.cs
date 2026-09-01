using SNS.Application.Discussions.Problems.Problems.Contracts;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Shared.Enums;

namespace SNS.Application.Search.Discussions.Problems.Queries.GetProblemsSearch;

/// <summary>
/// Represents a search query to search problem documents in the search index and return authoritative problem summaries.
/// </summary>
/// <param name="SearchTerm">Optional keyword or text query to search within problem details.</param>
/// <param name="MinCreatedAt">Optional minimum problem creation date filter.</param>
/// <param name="MaxCreatedAt">Optional maximum problem creation date filter.</param>
/// <param name="Level">Optional problem difficulty level filter.</param>
/// <param name="Status">Optional problem resolution status filter.</param>
/// <param name="Page">The page index for pagination (1-based).</param>
/// <param name="PageSize">The maximum number of problem documents to return per page.</param>
public sealed record GetProblemsSearchQuery(
    string? SearchTerm = null,
    DateTime? MinCreatedAt = null,
    DateTime? MaxCreatedAt = null,
    DifficultyLevel? Level = null,
    ProblemStatus? Status = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<SearchResult<ProblemSummaryDto>>;
