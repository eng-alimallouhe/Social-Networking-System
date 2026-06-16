using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.Users.Events;

public sealed record UserRegisteredEvent(
    string UserName,
    string Email,
    SupportedLanguage PreferredLanguage,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration) : IDomainEvent;
