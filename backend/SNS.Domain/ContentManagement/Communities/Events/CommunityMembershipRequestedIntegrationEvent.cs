using SNS.Domain.Shared.Events;

namespace SNS.Domain.ContentManagement.Communities.Events;

/// <summary>
/// Integration event raised when a profile requests to join a private community.
/// </summary>
public sealed record CommunityMembershipRequestedIntegrationEvent(
    Guid CommunityId,
    Guid SubmitterId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
