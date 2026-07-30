using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Application.ContentManagement.Posts.Contracts;

/// <summary>
/// Represents data transfer object containing summary overview details of a published post entity.
/// </summary>
/// <param name="Id">The unique identifier of the post.</param>
/// <param name="AuthorId">The profile ID of the post author.</param>
/// <param name="AuthorName">The full name of the author.</param>
/// <param name="AuthorSpecialization">Optional specialization of the author.</param>
/// <param name="AuthorProfilePictureUrl">Optional profile avatar URL of the author.</param>
/// <param name="CommunityId">Optional community ID where the post was published.</param>
/// <param name="CommunityType">Optional community type classification.</param>
/// <param name="CommunityName">Optional community name.</param>
/// <param name="CommunityLogoUrl">Optional community logo image URL.</param>
/// <param name="Title">The title header of the post.</param>
/// <param name="Content">The text content excerpt of the post.</param>
/// <param name="CreatedAt">The timestamp when the post was created.</param>
/// <param name="UpdatedAt">The timestamp when the post was last updated.</param>
/// <param name="LastInteractedAt">Optional timestamp of the latest interaction.</param>
/// <param name="FirstMediaUrl">Optional URL to the primary media attachment.</param>
/// <param name="MediaCount">Total number of media attachments on the post.</param>
/// <param name="Tags">List of topic hashtag strings.</param>
/// <param name="CommentsCount">Total count of comments on the post.</param>
/// <param name="ReactionsCount">Total count of reactions received on the post.</param>
/// <param name="ViewsCount">Total view count of the post.</param>
/// <param name="SavesCount">Total bookmark/save count of the post.</param>
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

