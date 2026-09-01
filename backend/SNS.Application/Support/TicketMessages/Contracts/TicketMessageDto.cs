namespace SNS.Application.Support.TicketMessages.Contracts;

public sealed record TicketMessageDto(
    Guid Id,
    Guid TicketId,
    Guid SenderId,
    bool IsFromAgent,
    string MessageBody,
    DateTime SentAt,
    IReadOnlyList<TicketAttachmentDto> Attachments);
