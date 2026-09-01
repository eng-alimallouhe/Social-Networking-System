using SNS.Domain.Support.Enums;

namespace SNS.Application.Support.SupportTickets.Contracts;

public sealed record ChangeSupportTicketStatusRequestDto(TicketStatus Status);
