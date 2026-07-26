using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.Users.Events;

public sealed record UserRoleChangedEvent(
    Guid UserId,
    string UserName,
    string OldRole,
    string NewRole,
    string Email,
    SupportedLanguage SendLanguage,
    CommunicationMethod SendMethod,
    DateTime OccurredOn,
    EventType EventType = EventType.Synchronous): IDomainEvent;
