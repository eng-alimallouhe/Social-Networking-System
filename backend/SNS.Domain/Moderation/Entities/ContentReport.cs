using SNS.Domain.Moderation.Enums;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Moderation.Entities;

public class ContentReport : Entity, IHardDeletable
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public Guid ReporterId { get; private set; }
    public ViolationReason ViolationReason { get; private set; }
    public string? AdditionalDetails { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ContentReport() 
    { 
        Id = SequentialGuid.GenerateSequentialGuid();
    } 

    public static ContentReport Create(Guid reporterId, ViolationReason violationReason, string? details)
    {
        return new ContentReport
        {
            Id = Guid.NewGuid(),
            ReporterId = reporterId,
            ViolationReason = violationReason,
            AdditionalDetails = details,
            CreatedAt = DateTime.UtcNow
        };
    }
}
