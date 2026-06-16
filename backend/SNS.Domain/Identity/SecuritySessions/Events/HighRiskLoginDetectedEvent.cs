using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.SecuritySessions.Events;

public sealed record HighRiskLoginDetectedEvent(
    Guid UserId,
    string UserName,
    string IpAddress,
    string Country,
    string City,
    double Latitude,
    double Longitude,
    string RecipientAddress,
    string Device,
    CommunicationMethod SendMethod,
    SupportedLanguage SendLanguage,
    DateTime OccurredOn,
    EventType EventType = EventType.Synchronous) : IDomainEvent;
