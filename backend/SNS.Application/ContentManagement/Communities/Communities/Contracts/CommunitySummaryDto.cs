using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Application.ContentManagement.Communities.Communities.Contracts;

/// <summary>
/// Represents summary community overview information for search, list views, and feeds.
/// </summary>
/// <param name="Id">The unique identifier of the community.</param>
/// <param name="Name">The name of the community.</param>
/// <param name="Description">The brief description of the community.</param>
/// <param name="Type">The privacy/visibility type of the community.</param>
/// <param name="LogoUrl">Optional resolved temporary logo URL.</param>
/// <param name="MembersCount">The total number of active members in the community.</param>
/// <param name="CreatedAt">The date and time when the community was created.</param>
public sealed record CommunitySummaryDto(
    Guid Id,
    string Name,
    string Description,
    CommunityType Type,
    string? LogoUrl,
    int MembersCount,
    DateTime CreatedAt
);
