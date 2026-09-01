using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;
using SNS.Domain.Support.Enums;
using SNS.Shared.Exceptions;

namespace SNS.Domain.Support.Entities;

public class SupportTicket : Entity, IHardDeletable
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

    // Conversation thread messages
    public ICollection<TicketMessage> Messages { get; private set; } = new List<TicketMessage>();

    private SupportTicket() { }

    public static SupportTicket Create(
        Guid userId,
        string title,
        SupportTeckitCategory category,
        TicketPriority priority,
        string initialMessage,
        IReadOnlyCollection<string>? attachmentObjectKeys = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Ticket title cannot be empty.");

        var ticket = new SupportTicket
        {
            Id = SequentialGuid.GenerateSequentialGuid(),
            UserId = userId,
            Title = title.Trim(),
            Category = category,
            Priority = priority,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        ticket.Messages.Add(TicketMessage.Create(ticket.Id, userId, isFromAgent: false, body: initialMessage, attachmentObjectKeys));
        return ticket;
    }

    public void AddUserReply(string messageBody, IReadOnlyCollection<string>? attachmentObjectKeys = null)
    {
        if (Status == TicketStatus.Closed)
            throw new DomainException("Cannot reply to a closed ticket.");

        var message = TicketMessage.Create(Id, UserId, isFromAgent: false, body: messageBody, attachmentObjectKeys);
        Messages.Add(message);

        Status = TicketStatus.Open; 
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAgentReply(Guid agentId, string messageBody, IReadOnlyCollection<string>? attachmentObjectKeys = null)
    {
        if (Status == TicketStatus.Closed)
            throw new DomainException("Cannot reply to a closed ticket.");

        AssignedAgentId = agentId; 
        var message = TicketMessage.Create(Id, agentId, isFromAgent: true, body: messageBody, attachmentObjectKeys);
        Messages.Add(message);

        Status = TicketStatus.Pending; 
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToAgent(Guid agentId)
    {
        AssignedAgentId = agentId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePriority(TicketPriority priority)
    {
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(TicketStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CloseTicket()
    {
        ChangeStatus(TicketStatus.Closed);
    }
}