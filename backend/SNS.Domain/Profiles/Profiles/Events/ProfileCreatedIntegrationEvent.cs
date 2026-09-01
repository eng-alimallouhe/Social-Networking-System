using SNS.Domain.Shared.Events;

namespace SNS.Domain.Profiles.Profiles.Events;

/// <summary>
/// Integration event raised when a new user profile is created.
/// </summary>
public sealed record ProfileCreatedIntegrationEvent(
    Guid ProfileId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
