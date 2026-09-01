using SNS.Domain.Shared.Events;

namespace SNS.Domain.ContentManagement.Comments.Events;

/// <summary>
/// Integration event raised when a comment is deleted.
/// </summary>
/// <param name="ProfileId">The identifier of the author profile whose reputation should be reversed.</param>
/// <param name="CommentId">The identifier of the deleted comment.</param>
/// <param name="PostId">The identifier of the post on which the comment existed.</param>
/// <param name="OccurredOn">The timestamp when the event occurred.</param>
/// <param name="EventType">The event dispatch type.</param>
public sealed record CommentDeletedIntegrationEvent(
    Guid ProfileId,
    Guid CommentId,
    Guid PostId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
