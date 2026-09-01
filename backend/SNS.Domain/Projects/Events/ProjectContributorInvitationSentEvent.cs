using SNS.Domain.Shared.Events;

namespace SNS.Domain.Projects.Events;

public record ProjectContributorInvitationSentEvent(
    string ProjectName,
    string ProjectOwnerName,
    string ProjectOwnerProfileImageUrl,
    Guid InvitedProfileId,
    DateTime OccurredOn) : IDomainEvent
{
    public EventType EventType => EventType.Integration;
}
