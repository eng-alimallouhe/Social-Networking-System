using SNS.Domain.Shared.Events;

namespace SNS.Domain.Jobs.Events;

/// <summary>
/// Integration event raised when a job posting is updated.
/// </summary>
public sealed record JobUpdatedIntegrationEvent(
    Guid JobId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
