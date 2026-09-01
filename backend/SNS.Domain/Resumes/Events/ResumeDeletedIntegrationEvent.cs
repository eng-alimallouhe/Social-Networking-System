using SNS.Domain.Shared.Events;

namespace SNS.Domain.Resumes.Events;

/// <summary>
/// Integration event raised when a resume is deleted.
/// </summary>
/// <param name="ProfileId">The owner profile identifier whose reputation should be reversed.</param>
/// <param name="ResumeId">The identifier of the deleted resume.</param>
/// <param name="OccurredOn">The timestamp when the event occurred.</param>
/// <param name="EventType">The event dispatch type.</param>
public sealed record ResumeDeletedIntegrationEvent(
    Guid ProfileId,
    Guid ResumeId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
