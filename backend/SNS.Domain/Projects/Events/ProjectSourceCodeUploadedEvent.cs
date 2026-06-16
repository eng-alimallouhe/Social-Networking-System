using SNS.Domain.Shared.Events;

namespace SNS.Domain.Projects.Events;

public sealed record ProjectSourceCodeUploadedEvent(
    Guid ProjectId,
    string TempZipObjectKey,
    DateTime OccurredOn,
    EventType EventType = EventType.Integration) : IDomainEvent;
