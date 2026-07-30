using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Shared.Enums;

namespace SNS.Application.Search.Discussions.Problems.Queries;

/// <summary>
/// Represents filter parameters to query and search problem documents in the search index.
/// </summary>
/// <param name="SearchTerm">Optional keyword or text query to search within problem details.</param>
/// <param name="MinCreatedAt">Optional minimum problem creation date filter.</param>
/// <param name="MaxCreatedAt">Optional maximum problem creation date filter.</param>
/// <param name="Level">Optional problem difficulty level filter.</param>
/// <param name="Status">Optional problem resolution status filter.</param>
/// <param name="Page">The page index for pagination (1-based).</param>
/// <param name="PageSize">The maximum number of problem documents to return per page.</param>
public sealed record ProblemSearchQuery(
    string? SearchTerm,
    DateTime? MinCreatedAt,
    DateTime? MaxCreatedAt,
    DifficultyLevel? Level,
    ProblemStatus? Status,
    int Page = 1,
    int PageSize = 10);

