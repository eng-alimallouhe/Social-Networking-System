using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.ContentManagement.Posts.PostMentions.Contracts;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.ContentManagement.Shared.Enums;

namespace SNS.Application.ContentManagement.Posts.Posts.Contracts;

/// <summary>
/// Represents data transfer object containing summary overview details of a published post.
/// </summary>
/// <param name="Id">The unique identifier of the post.</param>
/// <param name="Author">Lightweight profile snapshot of the author.</param>
/// <param name="Community">Optional community snapshot if published to a community.</param>
/// <param name="Title">The title header of the post.</param>
/// <param name="Content">The text content excerpt of the post.</param>
/// <param name="CreatedAt">The timestamp when the post was created.</param>
/// <param name="UpdatedAt">The timestamp when the post was last updated.</param>
/// <param name="LastInteractedAt">Optional timestamp of the latest interaction.</param>
/// <param name="Media">List of media attachments on the post with temporary URLs.</param>
/// <param name="Tags">List of topic hashtag strings.</param>
/// <param name="CommentsCount">Total count of comments on the post.</param>
/// <param name="ReactionsCount">Total count of reactions received on the post.</param>
/// <param name="ViewsCount">Total view count of the post.</param>
/// <param name="SavesCount">Total bookmark/save count of the post.</param>
/// <param name="CurrentUserReaction">The current user's reaction type, if any.</param>
/// <param name="Mentions">List of profiles mentioned in the post.</param>
public sealed record PostOverviewDto(
    Guid Id,
    ProfileSnapshotDto Author,
    CommunitySnapshotDto? Community,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? LastInteractedAt,
    List<PostMediaDto> Media,
    List<string> Tags,
    int CommentsCount,
    int ReactionsCount,
    int ViewsCount,
    int SavesCount,
    ReactionType? CurrentUserReaction,
    List<PostMentionDto> Mentions
);
