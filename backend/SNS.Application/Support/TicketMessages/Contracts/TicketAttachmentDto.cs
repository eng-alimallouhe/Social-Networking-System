namespace SNS.Application.Support.TicketMessages.Contracts;

public sealed record TicketAttachmentDto(
    Guid Id,
    string ObjectKey,
    string? PublicUrl,
    DateTime CreatedAt);
