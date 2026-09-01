using SNS.Domain.Shared.Events;

namespace SNS.Domain.ContentManagement.Posts.Events;

/// <summary>
/// Integration event raised when a new post is created.
/// </summary>
/// <param name="ProfileId">The identifier of the author profile whose reputation should be updated.</param>
/// <param name="PostId">The identifier of the created post.</param>
/// <param name="OccurredOn">The timestamp when the event occurred.</param>
/// <param name="EventType">The event dispatch type.</param>
public sealed record PostCreatedIntegrationEvent(
    Guid ProfileId,
    Guid PostId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
