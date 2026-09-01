using SNS.Domain.Shared.Events;

namespace SNS.Domain.Profiles.Profiles.Events;

/// <summary>
/// Integration event raised when profile searchable information is updated.
/// </summary>
public sealed record ProfileUpdatedIntegrationEvent(
    Guid ProfileId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
