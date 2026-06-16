using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.Users.Events;

public sealed record UserDeactivatedIntegrationEvent(
    Guid UserId,
    DateTime DeactivatedAt,
    string Device,
    string Browser,
    string Country,
    string IpAddress,
    DateTime OccurredOn,
    EventType EventType) : IDomainEvent;



public sealed record UserDeactivatedSynchronousEvent(
    Guid UserId,
    DateTime OccurredOn,
    EventType EventType = EventType.Synchronous) : IDomainEvent;
