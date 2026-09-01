using SNS.Domain.Shared.Events;

namespace SNS.Domain.ContentManagement.Communities.Events;

/// <summary>
/// Integration event raised when a community is deleted.
/// </summary>
public sealed record CommunityDeletedIntegrationEvent(
    Guid CommunityId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
