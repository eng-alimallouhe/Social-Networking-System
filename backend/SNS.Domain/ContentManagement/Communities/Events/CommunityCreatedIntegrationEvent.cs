using SNS.Domain.Shared.Events;

namespace SNS.Domain.ContentManagement.Communities.Events;

/// <summary>
/// Integration event raised when a new community is created.
/// </summary>
public sealed record CommunityCreatedIntegrationEvent(
    Guid CommunityId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
