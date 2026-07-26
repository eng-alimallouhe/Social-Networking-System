using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Application.ContentManagement.Posts.Contracts;

public sealed record PostOverviewDto(
    Guid Id,
    Guid AuthorId,
    string AuthorName,
    string? AuthorSpecialization,
    string? AuthorProfilePictureUrl,
    Guid? CommunityId,
    CommunityType? CommunityType,
    string? CommunityName,
    string? CommunityLogoUrl,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? LastInteractedAt,
    string FirstMediaUrl,
    int MediaCount,
    List<string> Tags,
    int CommentsCount,
    int ReactionsCount,
    int ViewsCount,
    int SavesCount
);
