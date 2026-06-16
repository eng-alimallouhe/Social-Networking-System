using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.Users.Events;

public sealed record UserBannedEvent(
    Guid UserId,
    string Email,
    SupportedLanguage UserLanguage,
    string Reason,
    CommunicationMethod CommunicationMethod,
    DateTime OccurredOn,
    EventType EventType = EventType.Synchronous) : IDomainEvent;

