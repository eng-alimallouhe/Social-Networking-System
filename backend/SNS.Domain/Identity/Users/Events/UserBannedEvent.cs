using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.Users.Events;

public sealed record UserBannedEvent(
    Guid UserId,
    string UserName,
    string Email,
    string Reason,
    SupportedLanguage SendLanguage,
    CommunicationMethod CommunicationMethod,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration) : IDomainEvent;

