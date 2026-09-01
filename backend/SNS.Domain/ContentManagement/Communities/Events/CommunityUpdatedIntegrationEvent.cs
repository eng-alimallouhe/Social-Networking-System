using SNS.Domain.Shared.Events;

namespace SNS.Domain.ContentManagement.Communities.Events;

/// <summary>
/// Integration event raised when community details are updated.
/// </summary>
public sealed record CommunityUpdatedIntegrationEvent(
    Guid CommunityId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
