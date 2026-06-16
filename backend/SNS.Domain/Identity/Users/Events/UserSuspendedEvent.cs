using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.Users.Events;

public sealed record UserSuspendedEvent(
    Guid UserId,
    string RecipientAddress,
    CommunicationMethod SendMethod,
    string UserName,
    string IpAddress,
    string City,
    string Country,
    double Latitude,
    double Longitude,
    string DeviceName,
    string SuspensionReason,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration) : IDomainEvent;
