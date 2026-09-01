using SNS.Domain.Shared.Events;

namespace SNS.Domain.Discussions.Problems.Events;

/// <summary>
/// Integration event raised when an existing discussion problem is updated.
/// </summary>
/// <param name="ProblemId">The unique identifier of the updated problem.</param>
/// <param name="RequiresReclassification">Indicates whether content changes require AI topic reclassification.</param>
/// <param name="OccurredOn">The timestamp when the event occurred.</param>
/// <param name="EventType">The classification of event dispatching.</param>
public sealed record ProblemUpdatedEvent(
    Guid ProblemId,
    bool RequiresReclassification,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
