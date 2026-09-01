using SNS.Application.Support.TicketMessages.Contracts;
using SNS.Domain.Support.Enums;

namespace SNS.Application.Support.SupportTickets.Contracts;

public sealed record SupportTicketDetailsDto(
    Guid Id,
    Guid UserId,
    Guid? AssignedAgentId,
    string Title,
    SupportTeckitCategory Category,
    TicketPriority Priority,
    TicketStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<TicketMessageDto> Messages);
