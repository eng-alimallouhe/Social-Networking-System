namespace SNS.Application.Support.TicketMessages.Contracts;

public sealed record ReplyToSupportTicketRequestDto(
    string MessageBody,
    IReadOnlyCollection<string>? AttachmentObjectKeys = null);
