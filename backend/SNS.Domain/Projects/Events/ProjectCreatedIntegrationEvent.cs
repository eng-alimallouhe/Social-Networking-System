using SNS.Domain.Shared.Events;

namespace SNS.Domain.Projects.Events;

/// <summary>
/// Integration event raised when a new project is created.
/// </summary>
public sealed record ProjectCreatedIntegrationEvent(
    Guid ProjectId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
