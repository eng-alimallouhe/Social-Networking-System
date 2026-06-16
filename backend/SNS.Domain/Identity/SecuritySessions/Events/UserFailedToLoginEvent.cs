using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.SecuritySessions.Events;

public sealed record UserFailedToLoginEvent(
    Guid UserId,
    string UserName,
    string IpAddress,
    string RecipientAddress,
    CommunicationMethod SendMethod,
    string DeviceName,
    double Longitude,
    double Latitude,
    string Country,
    string City,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration) : IDomainEvent;
