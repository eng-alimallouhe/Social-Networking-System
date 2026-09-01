using SNS.Application.Jobs.Companies.Contracts;
using SNS.Domain.Jobs.Enums;

namespace SNS.Application.Jobs.Jobs.Contracts;

public sealed record JobDetailsDto(
    Guid Id,
    Guid CompanyId,
    CompanySnapshotDto Company,
    string Title,
    string Description,
    string Location,
    JobType Type,
    decimal MinSalary,
    decimal MaxSalary,
    string CurrencyCode,
    SalaryType SalaryType,
    string KeyResponsibilitiesText,
    List<string> Skills,
    int ApplicationsCount,
    bool HasApplied,
    DateTime CreatedAt,
    DateTime? ClosedAt,
    bool IsActive
);
