using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.Users.Events;

public sealed record UserDeletedEvent(
    string Email,
    string UserName,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration) : IDomainEvent;
