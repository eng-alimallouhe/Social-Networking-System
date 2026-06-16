using SNS.Domain.Shared.Entities;
using SNS.Domain.Support.Enums;
using SNS.Shared.Exceptions;

namespace SNS.Domain.Support.Entities;


public class SupportTicket : Entity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; } 
    public Guid? AssignedAgentId { get; private set; } 

    public string Title { get; private set; } = string.Empty;
    public SupportTeckitCategory Category { get; private set; }
    public TicketPriority Priority { get; private set; }
    public TicketStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // شجرة الرسائل المتبادلة
    public ICollection<TicketMessage> Messages { get; private set; } = new List<TicketMessage>();

    private SupportTicket() { }

    public static SupportTicket Create(Guid userId, string title, SupportTeckitCategory category, TicketPriority priority, string initialMessage)
    {
        var ticket = new SupportTicket
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Category = category,
            Priority = priority,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        ticket.Messages.Add(TicketMessage.Create(userId, isFromAgent: false, body: initialMessage));
        return ticket;
    }

    public void AddUserReply(string messageBody, string? attachmentUrl = null)
    {
        if (Status == TicketStatus.Closed)
            throw new DomainException("Cannot reply to a closed ticket.");

        var message = TicketMessage.Create(UserId, isFromAgent: false, body: messageBody, attachmentUrl);
        Messages.Add(message);

        Status = TicketStatus.Open; 
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAgentReply(Guid agentId, string messageBody, string? attachmentUrl = null)
    {
        if (Status == TicketStatus.Closed)
            throw new DomainException("Cannot reply to a closed ticket.");

        AssignedAgentId = agentId; 
        var message = TicketMessage.Create(agentId, isFromAgent: true, body: messageBody, attachmentUrl);
        Messages.Add(message);

        Status = TicketStatus.Pending; 
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToAgent(Guid agentId)
    {
        AssignedAgentId = agentId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CloseTicket()
    {
        Status = TicketStatus.Closed;
        UpdatedAt = DateTime.UtcNow;
    }
}