using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.ContentManagement.Shared.Enums;

namespace SNS.Application.ContentManagement.Comments.CommentReactions.Contracts;

public sealed record CommentReactionSummaryDto(
    ProfileSnapshotDto User,
    ReactionType ReactionType,
    DateTime ReactedAt
);
