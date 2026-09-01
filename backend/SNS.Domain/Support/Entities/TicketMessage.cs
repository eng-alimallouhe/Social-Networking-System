using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;
using SNS.Shared.Exceptions;

namespace SNS.Domain.Support.Entities;

/// <summary>
/// Represents a message in a support ticket conversation thread.
/// </summary>
public class TicketMessage : Entity, IHardDeletable
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public Guid SenderId { get; private set; } 
    public bool IsFromAgent { get; private set; }
    public string MessageBody { get; private set; } = string.Empty;
    public DateTime SentAt { get; private set; }

    // Navigation
    public ICollection<TicketMessageAttachment> Attachments { get; private set; } = new List<TicketMessageAttachment>();

    private TicketMessage() { }

    public static TicketMessage Create(
        Guid ticketId,
        Guid senderId,
        bool isFromAgent,
        string body,
        IReadOnlyCollection<string>? attachmentObjectKeys = null)
    {
        if (string.IsNullOrWhiteSpace(body))
            throw new DomainException("Message body cannot be empty.");

        var message = new TicketMessage
        {
            Id = SequentialGuid.GenerateSequentialGuid(),
            TicketId = ticketId,
            SenderId = senderId,
            IsFromAgent = isFromAgent,
            MessageBody = body,
            SentAt = DateTime.UtcNow
        };

        if (attachmentObjectKeys != null)
        {
            foreach (var objectKey in attachmentObjectKeys)
            {
                if (!string.IsNullOrWhiteSpace(objectKey))
                {
                    message.Attachments.Add(TicketMessageAttachment.Create(message.Id, objectKey));
                }
            }
        }

        return message;
    }
}