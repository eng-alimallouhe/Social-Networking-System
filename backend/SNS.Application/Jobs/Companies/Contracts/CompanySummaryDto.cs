using SNS.Domain.Jobs.Enums;

namespace SNS.Application.Jobs.Companies.Contracts;

public sealed record CompanySummaryDto(
    Guid Id,
    string Name,
    string Industry,
    string? WebsiteUrl,
    string? LogoUrl,
    DateTime CreatedAt,
    int ActiveJobsCount,
    CompanyRole? CurrentUserRole
);
