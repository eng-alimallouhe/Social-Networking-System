using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Moderation.Enums;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Moderation.Entities;

public class ReportTicket : Entity, IHardDeletable
{
    public Guid Id { get; private set; }

    public Guid TargetId { get; private set; }
    public ReportTargetType TargetType { get; private set; }

    public TicketStatus Status { get; private set; }
    public int ReportCount { get; private set; }
    public Guid? ModeratorId { get; private set; } 
    public string? ModeratorNotes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation Property 
    public ICollection<ContentReport> Reports { get; private set; } 
        = new List<ContentReport>();
    
    public User? Moderator { get; private set; }

    private ReportTicket()
    { 
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static ReportTicket Create(Guid targetId, ReportTargetType targetType)
    {
        return new ReportTicket
        {
            Id = Guid.NewGuid(),
            TargetId = targetId,
            TargetType = targetType,
            Status = TicketStatus.Pending,
            ReportCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void AddReport(ContentReport report)
    {
        Reports.Add(report);
        ReportCount++;
        UpdatedAt = DateTime.UtcNow;
    }


    public void Resolve(TicketStatus finalStatus, Guid moderatorId, string? notes)
    {
        Status = finalStatus;
        ModeratorId = moderatorId;
        ModeratorNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
