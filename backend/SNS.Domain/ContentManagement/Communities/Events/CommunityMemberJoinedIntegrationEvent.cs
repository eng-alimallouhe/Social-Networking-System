using SNS.Domain.Shared.Events;

namespace SNS.Domain.ContentManagement.Communities.Events;

/// <summary>
/// Integration event raised when a profile joins a community (either directly in public community or upon request approval).
/// </summary>
public sealed record CommunityMemberJoinedIntegrationEvent(
    Guid CommunityId,
    Guid MemberId,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration
) : IDomainEvent;
