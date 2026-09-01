using SNS.Domain.Jobs.Enums;

namespace SNS.Application.Search.Jobs.Contracts;

/// <summary>
/// Represents summary job overview information for search and list views.
/// </summary>
public sealed record JobSummaryDto(
    Guid Id,
    string Title,
    string Description,
    string Location,
    JobType Type,
    decimal MinSalary,
    decimal MaxSalary,
    string CurrencyCode,
    SalaryType SalaryType,
    string CompanyName,
    DateTime CreatedAt,
    DateTime? ClosedAt
);
