namespace SNS.API.DTOs.Shared;

public sealed record SearchQueryFilter(
    string? SearchTerm,
    int CurrentPage = 1,
    int PageSize = 10
);