using SNS.Common.Enums;
using SNS.Common.Results;
using SNS.Domain.Common.Enums;

namespace SNS.Application.Abstractions.Messaging;

/// <summary>
/// Represents a domain service responsible for
/// locating and reading content templates for notifications.
/// 
/// This service encapsulates the business logic related to
/// template retrieval, internationalization resolution, and file system access, 
/// while keeping the Application layer decoupled from infrastructure and implementation details.
/// </summary>
public interface ITemplateReaderService
{
    // ------------------------------------------------------------------
    // Query operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Retrieves the content of a specific template based on language and purpose.
    /// 
    /// This method does not mutate state and is intended for
    /// read-only scenarios.
    /// </summary>
    /// <param name="language">
    /// The target language for the template (e.g., English, Arabic).
    /// </param>
    /// <param name="purpose">
    /// The specific use case for the template (e.g., EmailVerification, PasswordReset).
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the raw string content of the template
    /// if found; otherwise, an appropriate failure result (e.g., Template Not Found).
    /// </returns>
    Result<string?> ReadTemplate(SupportedLanguage language, SendPurpose purpose);
}