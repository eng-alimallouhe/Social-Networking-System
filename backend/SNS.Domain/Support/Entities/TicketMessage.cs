using SNS.Domain.Shared.Entities;
using SNS.Shared.Exceptions;

namespace SNS.Domain.Support.Entities;

public class TicketMessage : Entity
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public Guid SenderId { get; private set; } 
    public bool IsFromAgent { get; private set; }
    public string MessageBody { get; private set; } = string.Empty;
    public string? AttachmentUrl { get; private set; }
    public DateTime SentAt { get; private set; }

    private TicketMessage() { }

    public static TicketMessage Create(Guid senderId, bool isFromAgent, string body, string? attachmentUrl = null)
    {
        if (string.IsNullOrWhiteSpace(body))
            throw new DomainException("Message body cannot be empty.");

        return new TicketMessage
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            IsFromAgent = isFromAgent,
            MessageBody = body,
            AttachmentUrl = attachmentUrl,
            SentAt = DateTime.UtcNow
        };
    }
}