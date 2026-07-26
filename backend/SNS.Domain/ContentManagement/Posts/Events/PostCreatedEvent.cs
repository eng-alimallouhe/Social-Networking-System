using SNS.Domain.Shared.Events;

namespace SNS.Domain.ContentManagement.Posts.Events;

public sealed record PostCreatedEvent(
    Guid PostId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;

public sealed record PostUpdatedEvent(
    Guid PostId,
    bool RequiresReclassification,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;


