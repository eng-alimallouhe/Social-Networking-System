using SNS.Common.Results;

namespace SNS.Application.Abstractions.Messaging;

/// <summary>
/// Represents a domain service responsible for
/// dispatching short message service (SMS) notifications to users.
/// 
/// This service encapsulates the business logic related to
/// constructing and sending text messages, while keeping the Application layer
/// decoupled from infrastructure and implementation details (e.g., SMS gateways).
/// </summary>
public interface ISmsSenderService
{
    // ------------------------------------------------------------------
    // Command operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Sends an SMS message to the specified phone number.
    /// 
    /// This operation is responsible for:
    /// - Validating the phone number format (if applicable).
    /// - Dispatching the message content via the configured SMS provider.
    /// - Handling provider-specific communication or delivery failures.
    /// </summary>
    /// <param name="phoneNumber">
    /// The destination phone number.
    /// </param>
    /// <param name="message">
    /// The text content of the SMS message.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the operation
    /// completed successfully or failed with a business error.
    /// </returns>
    Task<Result> SendSmsAsync(string phoneNumber, string message);
}