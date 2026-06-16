using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.Users.Events;

public sealed record UserActivatedIntegrationEvent(
    Guid UserId,
    DateTime ActivatedAt,
    string Device,
    string Browser,
    string Country,
    string City,
    string IpAddress,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration) : IDomainEvent;

public sealed record UserActivatedSynchronousEvent(
    Guid UserId,
    DateTime ActivatedAt,
    string Device,
    string Browser,
    string Country,
    string IpAddress,
    DateTime OccurredOn,
    EventType EventType = EventType.Synchronous) : IDomainEvent;
