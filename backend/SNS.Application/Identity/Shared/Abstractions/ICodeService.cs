using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Shared.Results;

namespace SNS.Application.Identity.Shared.Abstractions;

/// <summary>
/// Represents a domain service responsible for
/// the lifecycle management and validation of verification codes (One-Time Passwords).
/// 
/// This service encapsulates the business logic related to
/// secure code generation, dispatching via notification channels, and validation, 
/// while keeping the Application layer decoupled from infrastructure and implementation details.
/// </summary>
public interface ICodeService
{
    // ------------------------------------------------------------------
    // Command operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Generates a new verification code and initiates the dispatch process.
    /// 
    /// This operation is responsible for:
    /// - Generating a secure random code associated with the user.
    /// - Applying throttling and expiration policies.
    /// - Formatting the message in the user's preferred language.
    /// - Dispatching the code to the user's communication channel (RecoveryEmail/RecoveryEmail).
    /// </summary>
    /// <param name="userIdentifier">
    /// The recipient's identifier (e.g., email address or userName) 
    /// where the code will be sent.
    /// </param>
    /// <param name="userId">
    /// The unique identifier of the user requesting the code.
    /// </param>
    /// <param name="codeType">
    /// The specific purpose of the code (e.g., <see cref="CodeType.PhoneNumberConfirmation"/>),
    /// used to categorize the verification attempt.
    /// </param>
    /// <param name="language">
    /// The preferred language used to localize the notification message.
    /// </param>
    /// <param name="pendingUpdateId">
    /// An optional identifier for a pending state change (e.g., a new registration or phone change).
    /// </param>
    /// <param name="redirectUrl">
    /// An optional URL included in the notification (e.g., for email magic links).
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the operation
    /// completed successfully or failed with a business error.
    /// </returns>
    Task<Result> SendCodeAsync(
        CodeSendRequest dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the authenticity and validity of a provided verification code.
    /// 
    /// This operation is responsible for:
    /// - Verifying the code matches the stored value.
    /// - Enforcing expiration times and maximum retry limits.
    /// - Marking the code as used upon success to prevent replay attacks.
    /// </summary>
    /// <param name="verifyCodeDto">
    /// The data transfer object containing the user identifier, code, and context.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the operation
    /// completed successfully or failed with a business error.
    /// </returns>
    Task<Result> VerifyCodeAsync(VerifyCodeDto verifyCodeDto, CancellationToken cancellationToken = default);
}
