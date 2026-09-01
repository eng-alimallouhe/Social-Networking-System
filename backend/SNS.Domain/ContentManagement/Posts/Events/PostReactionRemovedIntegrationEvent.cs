using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Domain.Shared.Events;

namespace SNS.Domain.ContentManagement.Posts.Events;

/// <summary>
/// Integration event raised when a reaction is removed from a post.
/// </summary>
/// <param name="AuthorProfileId">The author profile ID of the post whose reaction was removed.</param>
/// <param name="PostId">The identifier of the post.</param>
/// <param name="ReactionId">The unique identifier of the removed reaction.</param>
/// <param name="ReactorProfileId">The profile ID of the user who removed their reaction.</param>
/// <param name="ReactionType">The reaction type that was removed.</param>
/// <param name="OccurredOn">The timestamp when the event occurred.</param>
/// <param name="EventType">The event dispatch type.</param>
public sealed record PostReactionRemovedIntegrationEvent(
    Guid AuthorProfileId,
    Guid PostId,
    Guid ReactionId,
    Guid ReactorProfileId,
    ReactionType ReactionType,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
