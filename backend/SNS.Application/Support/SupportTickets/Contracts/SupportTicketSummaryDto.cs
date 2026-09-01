using SNS.Domain.Support.Enums;

namespace SNS.Application.Support.SupportTickets.Contracts;

public sealed record SupportTicketSummaryDto(
    Guid Id,
    Guid UserId,
    Guid? AssignedAgentId,
    string Title,
    SupportTeckitCategory Category,
    TicketPriority Priority,
    TicketStatus Status,
    int MessagesCount,
    DateTime CreatedAt,
    DateTime UpdatedAt);
