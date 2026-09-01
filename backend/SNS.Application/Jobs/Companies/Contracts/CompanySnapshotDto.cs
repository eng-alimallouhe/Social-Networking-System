namespace SNS.Application.Jobs.Companies.Contracts;

public sealed record CompanySnapshotDto(
    Guid Id,
    string Name,
    string Industry,
    string? WebsiteUrl,
    string? LogoUrl
);
