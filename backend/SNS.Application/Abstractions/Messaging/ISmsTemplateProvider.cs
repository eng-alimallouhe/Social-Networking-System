using SNS.Common.Enums;
using SNS.Common.Results;
using SNS.Domain.Common.Enums;

namespace SNS.Application.Abstractions.Messaging;

/// <summary>
/// Represents a domain service responsible for
/// retrieving structured SMS content templates.
/// 
/// This service encapsulates the business logic related to
/// loading localized SMS message bodies based on purpose and language,
/// while keeping the Application layer decoupled from infrastructure 
/// (like file systems or database lookups).
/// </summary>
public interface ISmsTemplateProvider
{
    /// <summary>
    /// Retrieves the raw body text for a specific SMS type in the requested language.
    /// 
    /// This method does not mutate state and is intended for
    /// read-only scenarios (template resolution).
    /// </summary>
    /// <param name="language">
    /// The target language for the SMS message (e.g., Arabic, English).
    /// </param>
    /// <param name="purpose">
    /// The specific business purpose or intent of the SMS (e.g., Verification, Alert).
    /// </param>
    /// <returns>
    /// The localized string content of the SMS body if found; otherwise, null.
    /// </returns>
    Result<string> GetSmsBody(
        SupportedLanguage language,
        SendPurpose purpose,
        IReadOnlyDictionary<string, string> replacements);
}