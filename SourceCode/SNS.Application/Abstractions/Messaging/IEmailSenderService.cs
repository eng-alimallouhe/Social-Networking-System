using SNS.Common.Results;

namespace SNS.Application.Abstractions.Messaging;

/// <summary>
/// Represents a domain service responsible for
/// dispatching email notifications to users.
/// 
/// This service encapsulates the business logic related to
/// constructing and sending emails, while keeping the Application layer
/// decoupled from infrastructure and implementation details (e.g., SMTP servers or 3rd party providers).
/// </summary>
public interface IEmailSenderService
{
    // ------------------------------------------------------------------
    // Command operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Sends an email message to the specified recipient.
    /// 
    /// This operation is responsible for:
    /// - Constructing the email payload with the provided subject and body.
    /// - Attempting to deliver the message via the configured email provider.
    /// - Handling provider-specific communication or failures.
    /// </summary>
    /// <param name="toEmail">
    /// The destination email address.
    /// </param>
    /// <param name="subject">
    /// The subject line of the email.
    /// </param>
    /// <param name="message">
    /// The body content of the email (typically HTML or plain text).
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the operation
    /// completed successfully or failed with a business error.
    /// </returns>
    Task<Result> SendEmailAsync(string toEmail, string subject, string message);
}