using SNS.Domain.Shared.Events;

namespace SNS.Domain.ContentManagement.Posts.Events;

/// <summary>
/// Integration event raised when a post is deleted.
/// </summary>
/// <param name="ProfileId">The identifier of the author profile whose reputation should be reversed.</param>
/// <param name="PostId">The identifier of the deleted post.</param>
/// <param name="OccurredOn">The timestamp when the event occurred.</param>
/// <param name="EventType">The event dispatch type.</param>
public sealed record PostDeletedIntegrationEvent(
    Guid ProfileId,
    Guid PostId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
