using SNS.Domain.Support.Enums;

namespace SNS.Application.Support.SupportTickets.Contracts;

public sealed record ChangeSupportTicketPriorityRequestDto(TicketPriority Priority);
