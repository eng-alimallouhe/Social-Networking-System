using SNS.Domain.Shared.Events;

namespace SNS.Domain.Discussions.Problems.Events;

/// <summary>
/// Integration event raised when a new discussion problem is created.
/// </summary>
public sealed record ProblemCreatedIntegrationEvent(
    Guid ProblemId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
