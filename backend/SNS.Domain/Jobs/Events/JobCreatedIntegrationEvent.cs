using SNS.Domain.Shared.Events;

namespace SNS.Domain.Jobs.Events;

/// <summary>
/// Integration event raised when a new job posting is created.
/// </summary>
public sealed record JobCreatedIntegrationEvent(
    Guid JobId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
