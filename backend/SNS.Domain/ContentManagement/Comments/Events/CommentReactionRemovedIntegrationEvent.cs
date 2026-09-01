using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Domain.Shared.Events;

namespace SNS.Domain.ContentManagement.Comments.Events;

/// <summary>
/// Integration event raised when a reaction is removed from a comment.
/// </summary>
/// <param name="AuthorProfileId">The author profile ID of the comment whose reaction was removed.</param>
/// <param name="CommentId">The identifier of the comment.</param>
/// <param name="ReactionId">The unique identifier of the removed reaction.</param>
/// <param name="ReactorProfileId">The profile ID of the user who removed their reaction.</param>
/// <param name="ReactionType">The reaction type that was removed.</param>
/// <param name="OccurredOn">The timestamp when the event occurred.</param>
/// <param name="EventType">The event dispatch type.</param>
public sealed record CommentReactionRemovedIntegrationEvent(
    Guid AuthorProfileId,
    Guid CommentId,
    Guid ReactionId,
    Guid ReactorProfileId,
    ReactionType ReactionType,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
