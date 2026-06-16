namespace SNS.Application.Search.ContentManagement.Posts.Queries;

public sealed record PostSearchQuery(
    string? SearchTerm,
    DateTime? MinCreatedAt,
    DateTime? MaxCreatedAt,
    int Page = 1,
    int PageSize = 10);
