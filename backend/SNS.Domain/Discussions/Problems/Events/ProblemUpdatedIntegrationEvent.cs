using SNS.Domain.Shared.Events;

namespace SNS.Domain.Discussions.Problems.Events;

/// <summary>
/// Integration event raised when a discussion problem's searchable details are updated.
/// </summary>
public sealed record ProblemUpdatedIntegrationEvent(
    Guid ProblemId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
