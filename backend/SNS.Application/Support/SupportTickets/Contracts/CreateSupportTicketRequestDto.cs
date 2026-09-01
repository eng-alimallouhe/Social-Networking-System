using SNS.Domain.Support.Enums;

namespace SNS.Application.Support.SupportTickets.Contracts;

public sealed record CreateSupportTicketRequestDto(
    string Title,
    SupportTeckitCategory Category,
    TicketPriority Priority,
    string InitialMessage,
    IReadOnlyCollection<string>? AttachmentObjectKeys = null);
