namespace SNS.Application.Shared.Contracts.Messaging;

/// <summary>
/// Represents email content model containing subject line and body content.
/// </summary>
/// <param name="Subject">The subject line of the email.</param>
/// <param name="Body">The main body content of the email.</param>
public record EmailContent(
    string Subject,
    string Body);

