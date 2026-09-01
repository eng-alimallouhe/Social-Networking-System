using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.ContentManagement.Shared.Enums;

namespace SNS.Application.ContentManagement.Posts.PostReactions.Contracts;

/// <summary>
/// Represents data transfer object summarizing a user reaction to a post.
/// </summary>
/// <param name="User">Snapshot of the user profile who reacted.</param>
/// <param name="ReactionType">The type of reaction applied.</param>
/// <param name="ReactedAt">The timestamp when the reaction was recorded.</param>
public sealed record PostReactionSummaryDto(
    ProfileSnapshotDto User,
    ReactionType ReactionType,
    DateTime ReactedAt
);
