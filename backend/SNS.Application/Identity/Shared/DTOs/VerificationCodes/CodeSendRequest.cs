using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Shared.DTOs.VerificationCodes;

/// <summary>
/// Represents internal request parameters for generating and dispatching a verification code message.
/// </summary>
/// <param name="UserId">The recipient user identifier.</param>
/// <param name="UserName">The recipient username for message template personalization.</param>
/// <param name="RecipientAddress">The email address or phone number recipient destination.</param>
/// <param name="Purpose">The business purpose of sending the code (e.g. Verification, PasswordReset).</param>
/// <param name="SendMethod">The delivery channel (e.g. Email, SMS).</param>
/// <param name="SendLanguage">The language format for the verification message.</param>
/// <param name="RedirectUrl">The callback redirect URL embedded in the message.</param>
/// <param name="Token">The verification token associated with the code request.</param>
public sealed record CodeSendRequest(
    Guid UserId,
    string UserName,
    string RecipientAddress,
    SendPurpose Purpose,
    CommunicationMethod SendMethod,
    SupportedLanguage SendLanguage,
    string RedirectUrl,
    string Token);

