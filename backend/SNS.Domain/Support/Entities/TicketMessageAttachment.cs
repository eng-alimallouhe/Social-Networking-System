using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;
using SNS.Shared.Exceptions;

namespace SNS.Domain.Support.Entities;

/// <summary>
/// Represents an attachment associated with a support ticket message.
/// </summary>
public class TicketMessageAttachment : Entity, IHardDeletable
{
    public Guid Id { get; private set; }
    public Guid TicketMessageId { get; private set; }
    public string ObjectKey { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private TicketMessageAttachment() { }

    public static TicketMessageAttachment Create(Guid ticketMessageId, string objectKey)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
            throw new DomainException("Attachment object key cannot be empty.");

        return new TicketMessageAttachment
        {
            Id = SequentialGuid.GenerateSequentialGuid(),
            TicketMessageId = ticketMessageId,
            ObjectKey = objectKey.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
