using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Security.Entities;

public class SupportResponse : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(User) To Many(SupportResponses)
    public Guid SenderId { get; set; }

    // Foreign Key: One(SupportTicket) To Many(Responses)
    public Guid TicketId { get; set; }

    // Self-referencing Foreign Key: One(Response) To Many(Replies)
    public Guid? ParentResponseId { get; set; }

    public required string Message { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }

    public List<string> Attachments { get; set; } = new List<string>();
    public bool IsFromSupport { get; set; }

    // Navigation
    public ICollection<SupportResponse> Replies { get; set; } = new List<SupportResponse>();


    public SupportResponse()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}