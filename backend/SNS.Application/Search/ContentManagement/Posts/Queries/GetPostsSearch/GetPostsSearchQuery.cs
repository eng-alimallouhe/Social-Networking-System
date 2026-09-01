using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Search.ContentManagement.Posts.Queries.GetPostsSearch;

/// <summary>
/// Represents a search query to search post documents in the search index and return authoritative post overviews.
/// </summary>
/// <param name="SearchTerm">Optional keyword or phrase to search within post title, content, tags, or topics.</param>
/// <param name="MinCreatedAt">Optional minimum post creation timestamp filter.</param>
/// <param name="MaxCreatedAt">Optional maximum post creation timestamp filter.</param>
/// <param name="Tags">Optional list of tags to filter posts.</param>
/// <param name="Topics">Optional list of topics to filter posts.</param>
/// <param name="Page">The page index for pagination (1-based).</param>
/// <param name="PageSize">The maximum number of post records to return per page.</param>
public sealed record GetPostsSearchQuery(
    string? SearchTerm = null,
    DateTime? MinCreatedAt = null,
    DateTime? MaxCreatedAt = null,
    List<string>? Tags = null,
    List<string>? Topics = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<SearchResult<PostOverviewDto>>;
