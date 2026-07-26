using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.SecuritySessions.Events;

public sealed record UserLoggedInEvent(
    Guid UserId,
    Guid SessionId,
    string IpAddress,
    string Device,
    string City,
    string Country,
    double Latitude,
    double Longitude,
    string RecipientAddress,
    SupportedLanguage UserLanguage,
    CommunicationMethod SendMethod,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration) : IDomainEvent;
