using SNS.Domain.Shared.Events;

namespace SNS.Domain.Projects.Events;

public record ProjectContributorInvitationRespondedEvent(
    Guid ProjectId,
    Guid ProjectOwnerProfileId,
    string InvitedUserName,
    bool IsAccepted,
    DateTime OccurredOn) : IDomainEvent
{
    public EventType EventType => EventType.Integration;
}
