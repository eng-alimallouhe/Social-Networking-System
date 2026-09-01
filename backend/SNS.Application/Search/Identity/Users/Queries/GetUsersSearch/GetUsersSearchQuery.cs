using SNS.Application.Identity.Shared.DTOs.Users;
using SNS.Application.Search.Identity.Users.Queries;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Search.Identity.Users.Queries.GetUsersSearch;

/// <summary>
/// Represents a search query to search user documents in the search index using specified filter criteria.
/// </summary>
/// <param name="SearchTerm">Optional keyword to search within user account information.</param>
/// <param name="Role">Optional system role filter.</param>
/// <param name="IsBanned">Optional ban status filter.</param>
/// <param name="IsActive">Optional account activity status filter.</param>
/// <param name="IsSuspended">Optional account suspension status filter.</param>
/// <param name="IsVerified">Optional account email verification status filter.</param>
/// <param name="Page">The page index for pagination (1-based).</param>
/// <param name="PageSize">The maximum number of user records to return per page.</param>
/// <param name="SortDirection">Sorting order direction (Ascending or Descending).</param>
/// <param name="SortBy">Field by which results should be ordered.</param>
/// <param name="MinCreatedAt">Optional minimum account registration date filter.</param>
/// <param name="MaxCreatedAt">Optional maximum account registration date filter.</param>
/// <param name="MinLastLogin">Optional minimum last login timestamp filter.</param>
/// <param name="MaxLastLogin">Optional maximum last login timestamp filter.</param>
public sealed record GetUsersSearchQuery(
    string? SearchTerm = null,
    string? Role = null,
    bool? IsBanned = null,
    bool? IsActive = null,
    bool? IsSuspended = null,
    bool? IsVerified = null,
    int Page = 1,
    int PageSize = 10,
    SortDirection SortDirection = SortDirection.Descending,
    UserSearchSortBy SortBy = UserSearchSortBy.LastLogin,
    DateTime? MinCreatedAt = null,
    DateTime? MaxCreatedAt = null,
    DateTime? MinLastLogin = null,
    DateTime? MaxLastLogin = null
) : IQuery<SearchResult<UserSummaryDto>>;
