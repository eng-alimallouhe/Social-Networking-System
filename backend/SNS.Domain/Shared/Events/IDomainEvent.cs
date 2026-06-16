namespace SNS.Domain.Shared.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
    EventType EventType { get; }
}

public enum EventType
{
    Integration,
    Synchronous
}
