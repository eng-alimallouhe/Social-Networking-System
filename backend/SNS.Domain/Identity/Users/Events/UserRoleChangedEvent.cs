using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.Users.Events;

public sealed record UserRoleChangedEvent(
    Guid UserId,
    string RoleName,
    string Email,
    DateTime OccurredOn,
    EventType EventType = EventType.Synchronous): IDomainEvent;
