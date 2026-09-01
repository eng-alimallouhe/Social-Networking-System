using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.Users.Events;

/// <summary>
/// Integration event raised when a new user account is created.
/// </summary>
public sealed record UserCreatedIntegrationEvent(
    Guid UserId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
