using SNS.Domain.QA.Enums;

namespace SNS.Application.Search.Jobs.Queries.GetJobsSearch;

public sealed record JobSearchQuery(
    string? SearchTerm = null,
    JobType? Type = null,
    SalaryType? SalaryType = null,
    decimal? MinSalary = null,
    decimal? MaxSalary = null,
    DateTime? MinCreatedAt = null,
    DateTime? MaxCreatedAt = null,
    int Page = 1,
    int PageSize = 10
);
