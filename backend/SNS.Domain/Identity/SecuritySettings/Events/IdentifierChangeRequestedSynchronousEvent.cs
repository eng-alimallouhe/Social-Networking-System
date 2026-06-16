using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Shared.Events;

namespace SNS.Domain.Identity.SecuritySettings.Events;

public sealed record IdentifierChangeRequestedSynchronousEvent(
    Guid UserId,
    string UserName, 
    string Device,
    string City,
    string Country,
    double Latitude,
    double Longitude,
    string Browser,
    string RecipientAddress,
    CommunicationMethod DefaultCommunicationMethod,
    UpdateType UpdateType,
    EventType EventType,
    DateTime OccurredOn) : IDomainEvent;
