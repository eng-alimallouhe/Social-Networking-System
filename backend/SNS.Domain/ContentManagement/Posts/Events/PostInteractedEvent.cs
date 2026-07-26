using SNS.Domain.ContentManagement.Posts.Enums;
using SNS.Domain.Shared.Events;

namespace SNS.Domain.ContentManagement.Posts.Events;

public sealed record PostInteractedEvent(
    Guid ProfileId,
    Guid PostId,
    InteractionType Type,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;