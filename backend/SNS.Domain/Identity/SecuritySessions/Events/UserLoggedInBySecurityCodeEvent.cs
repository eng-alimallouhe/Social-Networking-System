using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.SecuritySessions.Events;

public sealed record UserLoggedInBySecurityCodeEvent(
    Guid UserId,
    Guid SessionId,
    string IpAddress,
    string Device,
    string City,
    string Country,
    double Latitude,
    double Longitude,
    SupportedLanguage UserLanguage,
    string RecipientAddress,
    CommunicationMethod SendMethod,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration) : IDomainEvent;
