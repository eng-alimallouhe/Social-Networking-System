using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.Users.Events;

/// <summary>
/// Integration event raised when a user account's searchable properties are updated.
/// </summary>
public sealed record UserUpdatedIntegrationEvent(
    Guid UserId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
