using SNS.Application.Search.Shared.Contracts;

namespace SNS.Application.Search.Identity.Users.Queries;

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
