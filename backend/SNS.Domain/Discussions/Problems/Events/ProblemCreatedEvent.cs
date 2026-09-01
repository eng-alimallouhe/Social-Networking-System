using SNS.Domain.Shared.Events;

namespace SNS.Domain.Discussions.Problems.Events;

/// <summary>
/// Integration event raised when a new discussion problem is created.
/// </summary>
/// <param name="ProblemId">The unique identifier of the created problem.</param>
/// <param name="OccurredOn">The timestamp when the event occurred.</param>
/// <param name="EventType">The classification of event dispatching.</param>
public sealed record ProblemCreatedEvent(
    Guid ProblemId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
