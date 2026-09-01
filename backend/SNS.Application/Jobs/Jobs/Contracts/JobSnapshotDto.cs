using SNS.Domain.Jobs.Enums;

namespace SNS.Application.Jobs.Jobs.Contracts;

public sealed record JobSnapshotDto(
    Guid Id,
    string Title,
    string CompanyName,
    string Location,
    JobType Type,
    decimal MinSalary,
    decimal MaxSalary,
    string CurrencyCode,
    SalaryType SalaryType
);
