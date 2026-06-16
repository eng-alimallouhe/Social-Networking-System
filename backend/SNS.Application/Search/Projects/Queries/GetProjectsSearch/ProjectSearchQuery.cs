using SNS.Domain.Projects.Enums;

namespace SNS.Application.Search.Projects.Queries.GetProjectsSearch;

public sealed record ProjectSearchQuery(
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
);
