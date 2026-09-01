using SNS.Domain.Jobs.Enums;

namespace SNS.Application.Jobs.Companies.Contracts;

public sealed record CompanyDetailsDto(
    Guid Id,
    string Name,
    string Industry,
    string? WebsiteUrl,
    string? LogoUrl,
    DateTime CreatedAt,
    bool IsActive,
    int ActiveJobsCount,
    int AdministratorsCount,
    CompanyRole? CurrentUserRole
);
