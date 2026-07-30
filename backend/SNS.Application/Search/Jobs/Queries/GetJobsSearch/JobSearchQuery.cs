using SNS.Domain.Jobs.Enums;
using SNS.Domain.QA.Enums;

namespace SNS.Application.Search.Jobs.Queries.GetJobsSearch;

/// <summary>
/// Represents filter parameters to query and search job posting documents in the search index.
/// </summary>
/// <param name="SearchTerm">Optional keyword or phrase to search within job title and description.</param>
/// <param name="Type">Optional job employment type filter (e.g. FullTime, PartTime).</param>
/// <param name="SalaryType">Optional salary payment structure filter.</param>
/// <param name="MinSalary">Optional minimum salary range filter.</param>
/// <param name="MaxSalary">Optional maximum salary range filter.</param>
/// <param name="MinCreatedAt">Optional minimum job posting creation date filter.</param>
/// <param name="MaxCreatedAt">Optional maximum job posting creation date filter.</param>
/// <param name="Page">The page index for pagination (1-based).</param>
/// <param name="PageSize">The maximum number of job postings to return per page.</param>
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

