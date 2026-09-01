using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.ContentManagement.Shared.Enums;

namespace SNS.Application.ContentManagement.Comments.Comments.Contracts;

public sealed record CommentSummaryDto(
    Guid Id,
    Guid PostId,
    Guid? ParentCommentId,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    ProfileSnapshotDto Author,
    int ReactionsCount,
    int RepliesCount,
    ReactionType? CurrentUserReaction
);
