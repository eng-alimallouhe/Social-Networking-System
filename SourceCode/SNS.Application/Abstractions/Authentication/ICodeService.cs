using SNS.Application.DTOs.Authentication.Verification.Requests;
using SNS.Common.Results;
using SNS.Domain.Security;

namespace SNS.Application.Abstractions.Authentication;

/// <summary>
/// Defines a domain-level service responsible for the lifecycle management
/// of verification codes (One-Time Passwords) used for authentication 
/// and security validations.
/// 
/// This service handles generating secure random codes, dispatching
/// them via the appropriate channels (Email/SMS), applying security 
/// policies (throttling, max retries), and validating user inputs.
/// 
/// All operations return a <see cref="Result"/> object, ensuring
/// a consistent error handling flow without exceptions.
/// </summary>
public interface ICodeService
{
    /// <summary>
    /// Generates a new verification code and dispatches it to the
    /// specified user identifier.
    /// 
    /// The code is stored with an expiration time, a failure counter,
    /// and is associated with the specific <see cref="CodeType"/>.
    /// </summary>
    /// <param name="userIdentifier">
    /// The recipient's identifier (e.g., email address or phone number)
    /// where the code will be sent.
    /// </param>
    /// <param name="codeType">
    /// The purpose of the code (e.g., <see cref="CodeType.PhoneNumberConfirmation"/>),
    /// used to categorize the verification attempt.
    /// </param>
    /// <param name="pendingUpdateId">
    /// An optional identifier for a <c>PendingUpdate</c> operation.
    /// Providing this links the code specifically to a staging operation
    /// (like a new registration or a phone change request) rather than
    /// just the user account.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> describing the outcome of the operation.
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// On success, implies the code was generated and added to the repository
    /// (but not necessarily committed to DB yet), and handed off to the dispatcher.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// On failure, returns a specific error (e.g., "Too many attempts",
    /// "User not found", or dispatch failure).
    /// </description>
    /// </item>
    /// </list>
    /// </returns>
    Task<Result> SendCodeAsync(string userIdentifier, CodeType codeType, Guid? pendingUpdateId = null);

    /// <summary>
    /// Validates a verification code provided by the user.
    /// 
    /// Checks existence, expiration, and enforces maximum retry attempts.
    /// Increments the failure counter on invalid attempts.
    /// </summary>
    /// <param name="verifyCodeDto">
    /// An object containing the user identifier, the code to check,
    /// and the context type.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the verification passed.
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// On success, the code is marked as <c>IsUsed</c>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// On failure, returns an error indicating invalid code, expired code,
    /// or "Max attempts reached". The failure counter is persisted regardless
    /// of the result.
    /// </description>
    /// </item>
    /// </list>
    /// </returns>
    Task<Result> VerifyCodeAsync(VerifyCodeDto verifyCodeDto);

    /// <summary>
    /// Re-issues a new verification code for a user who did not
    /// receive or lost the previous one.
    /// 
    /// This process follows the same security throttling rules as
    /// <see cref="SendCodeAsync"/>.
    /// </summary>
    /// <param name="userIdentifier">
    /// The recipient's identifier (e.g., email address or phone number).
    /// </param>
    /// <param name="codeType">
    /// The purpose of the code being resent.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> describing the outcome.
    /// </returns>
    Task<Result> ResendCodeAsync(string userIdentifier, CodeType codeType);
}