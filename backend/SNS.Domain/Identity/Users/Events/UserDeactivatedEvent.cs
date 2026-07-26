using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.Users.Events;

public sealed record UserDeactivatedEvent(
    Guid UserId,
    string Email,
    string UserName,
    CommunicationMethod SendMethod,
    SupportedLanguage SendLanguage,
    string Device,
    string Browser,
    string Country,
    string City,
    double Longitude,
    double Latitude,
    string IpAddress,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration) : IDomainEvent;