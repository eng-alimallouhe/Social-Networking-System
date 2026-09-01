using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.Jobs.Enums;

namespace SNS.Application.Jobs.CompanyCreateRequests.Contracts;

public sealed record CompanyCreateRequestDetailsDto(
    Guid Id,
    Guid ProfileId,
    ProfileSnapshotDto Profile,
    string Name,
    string Industry,
    string? WebsiteUrl,
    string? LogoUrl,
    CompanyCreateRequestStatus Status,
    Guid? CreatedCompanyId,
    Guid? ReviewedByProfileId,
    string? ReviewNote,
    DateTime CreatedAt,
    DateTime? ReviewedAt
);
