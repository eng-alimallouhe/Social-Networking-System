using SNS.Domain.Shared.Events;

namespace SNS.Domain.ContentManagement.Comments.Events;

/// <summary>
/// Integration event raised when a new comment is created.
/// </summary>
/// <param name="ProfileId">The identifier of the author profile whose reputation should be updated.</param>
/// <param name="CommentId">The identifier of the created comment.</param>
/// <param name="PostId">The identifier of the post on which the comment was created.</param>
/// <param name="OccurredOn">The timestamp when the event occurred.</param>
/// <param name="EventType">The event dispatch type.</param>
public sealed record CommentCreatedIntegrationEvent(
    Guid ProfileId,
    Guid CommentId,
    Guid PostId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
