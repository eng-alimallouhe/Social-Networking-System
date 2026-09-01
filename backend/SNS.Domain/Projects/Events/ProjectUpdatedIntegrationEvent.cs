using SNS.Domain.Shared.Events;

namespace SNS.Domain.Projects.Events;

/// <summary>
/// Integration event raised when project searchable details are updated.
/// </summary>
public sealed record ProjectUpdatedIntegrationEvent(
    Guid ProjectId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
