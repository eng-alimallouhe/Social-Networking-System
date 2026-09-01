using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Application.ContentManagement.Communities.Communities.Contracts;

/// <summary>
/// Represents a lightweight snapshot of community information for embedding in posts, comments, search, and consuming features.
/// </summary>
/// <param name="Id">The unique identifier of the community.</param>
/// <param name="Name">The name of the community.</param>
/// <param name="Type">The privacy/visibility type of the community.</param>
/// <param name="LogoUrl">Optional resolved temporary logo URL.</param>
public sealed record CommunitySnapshotDto(
    Guid Id,
    string Name,
    CommunityType Type,
    string? LogoUrl
);
