using SNS.Application.Search.Shared.Contracts;

namespace SNS.Application.Search.Identity.Users.Queries;

/// <summary>
/// Represents filter and sorting parameters to query user documents in the search index.
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
public sealed record UserSearchQuery(
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

);

