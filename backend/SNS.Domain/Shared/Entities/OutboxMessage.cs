using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Shared.Entities;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; }

    // We will store the exact C# Type name here so we can deserialize it later
    public string Type { get; private set; } = string.Empty;

    // The JSON payload
    public string Content { get; private set; } = string.Empty;

    public DateTime OccurredOnUtc { get; private set; } = DateTime.UtcNow;

    public DateTime? ProcessedOnUtc { get; private set; }

    public string? Error { get; private set; }

    private OutboxMessage()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static OutboxMessage Create(string type, string content)
    {
        var entity = new OutboxMessage();
        entity.Type = type;
        entity.Content = content;
        return entity;
    }

    public void MarkProcessed()
    {
        this.ProcessedOnUtc = DateTime.UtcNow;
    }

    public void MarkFailed(string error)
    {
        this.Error = error;
        this.ProcessedOnUtc = DateTime.UtcNow;
    }
}
