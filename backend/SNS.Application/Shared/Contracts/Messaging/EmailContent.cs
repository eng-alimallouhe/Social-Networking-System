namespace SNS.Application.Shared.Contracts.Messaging;

public record EmailContent(
    string Subject,
    string Body);
