namespace SNS.Domain.Security;

public class SupportTicket
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(User) To Many(SupportTickets)
    public Guid ApplicantId { get; set; }

    public SupportTicketType Type { get; set; }
    public required string Description { get; set; }
    public List<string> Attachments { get; set; } = new List<string>();

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public Guid? ClosedBy { get; set; }
    public SupportTicketStatus Status { get; set; }

    // Navigation
    public ICollection<SupportResponse> Responses { get; set; } = new List<SupportResponse>();
}
