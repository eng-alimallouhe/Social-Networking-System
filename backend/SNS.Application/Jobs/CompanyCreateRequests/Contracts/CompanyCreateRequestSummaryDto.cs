using SNS.Domain.Jobs.Enums;

namespace SNS.Application.Jobs.CompanyCreateRequests.Contracts;

public sealed record CompanyCreateRequestSummaryDto(
    Guid Id,
    Guid ProfileId,
    string SubmitterName,
    string? SubmitterAvatarUrl,
    string Name,
    string Industry,
    string? WebsiteUrl,
    string? LogoUrl,
    CompanyCreateRequestStatus Status,
    DateTime CreatedAt,
    DateTime? ReviewedAt
);
