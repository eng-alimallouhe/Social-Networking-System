using SNS.Application.Shared.Contracts.Messaging;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Abstractions.Messaging;

/// <summary>
/// Represents a domain service responsible for
/// locating and reading content templates for notifications.
/// 
/// This service encapsulates the business logic related to
/// template retrieval, internationalization resolution, and file system access, 
/// while keeping the Application layer decoupled from infrastructure and implementation details.
/// </summary>
public interface IEmailTemplateProvider
{
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
    /// <param name="replacements">
    /// define the dynamic placeholders and their corresponding values to be injected into the template content.
    /// </param>
    /// <returns>
    /// A <see cref="EmailContent"/> containing the raw string content of the template and the subject, with placeholders for dynamic data.
    /// </returns>
    Task<EmailContent> ReadTemplate(
        SupportedLanguage language, 
        SendPurpose purpose,
        IReadOnlyList<MessageReplacement> replacements);
}
