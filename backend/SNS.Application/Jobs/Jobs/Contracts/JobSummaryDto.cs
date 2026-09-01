using SNS.Domain.Jobs.Enums;

namespace SNS.Application.Jobs.Jobs.Contracts;

public sealed record JobSummaryDto(
    Guid Id,
    Guid CompanyId,
    string CompanyName,
    string? CompanyLogoUrl,
    string Title,
    string Description,
    string Location,
    JobType Type,
    decimal MinSalary,
    decimal MaxSalary,
    string CurrencyCode,
    SalaryType SalaryType,
    int ApplicationsCount,
    List<string> Skills,
    DateTime CreatedAt,
    DateTime? ClosedAt,
    bool IsActive
);
