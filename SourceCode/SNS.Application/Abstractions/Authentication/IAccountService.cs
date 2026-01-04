using SNS.Application.DTOs.Authentication.Account.Requests;
using SNS.Application.DTOs.Authentication.Account.Responses;
using SNS.Application.DTOs.Authentication.Common.Responses;
using SNS.Application.DTOs.Authentication.Login.Requests;
using SNS.Application.DTOs.Authentication.Password.Requests;
using SNS.Application.DTOs.Authentication.TwoFactor.Requests;
using SNS.Common.Results;

namespace SNS.Application.Abstractions.Authentication;

/// <summary>
/// Represents a domain service responsible for
/// managing user account security, session lifecycles, and identity updates.
/// 
/// This service encapsulates the business logic related to
/// authentication (login/logout), credential management (password resets), 
/// and critical account modifications, while keeping the Application layer
/// decoupled from infrastructure and implementation details.
/// </summary>
public interface IAccountService
{
    // ------------------------------------------------------------------
    // Command operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Authenticates a user based on provided credentials and establishes a session.
    /// 
    /// This operation is responsible for:
    /// - Validating the username and password.
    /// - Checking account status (e.g., locked, banned, or requiring 2FA).
    /// - Generating authentication tokens upon success.
    /// </summary>
    /// <param name="loginDto">
    /// The data transfer object containing the user's credentials.
    /// </param>
    /// <returns>
    /// A <see cref="Result{AuthTokensDto}"/> containing the Access and Refresh tokens
    /// if authentication is successful; otherwise, a failure result.
    /// </returns>
    Task<Result<AuthTokensDto>> LoginAsync(LoginDto loginDto);

    /// <summary>
    /// Terminates the user's active session.
    /// 
    /// This operation is responsible for:
    /// - Revoking the current refresh token.
    /// - Marking the session as inactive to prevent further access.
    /// </summary>
    /// <param name="refreshToken">
    /// The token used to refresh authentication, identifying the session to end.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the operation
    /// completed successfully.
    /// </returns>
    Task<Result> LogoutAsync(string refreshToken);

    /// <summary>
    /// Updates the password for an already authenticated user.
    /// 
    /// This operation is responsible for:
    /// - Verifying the user's current password.
    /// - Enforcing password complexity policies on the new password.
    /// - Hashing and persisting the new credential.
    /// - Issuing new tokens to reflect the security change.
    /// </summary>
    /// <param name="changePasswordDto">
    /// The data transfer object containing old and new passwords.
    /// </param>
    /// <returns>
    /// A <see cref="Result{AuthTokensDto}"/> containing new tokens if the password change was successful.
    /// </returns>
    Task<Result<AuthTokensDto>> ChangePasswordAsync(ChangePasswordDto changePasswordDto);

    /// <summary>
    /// Initiates the password recovery process for a user who cannot log in.
    /// 
    /// This operation is responsible for:
    /// - Verifying that the provided identifier matches an existing account.
    /// - Generating a secure recovery code.
    /// - Dispatching the code via the user's registered communication channel.
    /// </summary>
    /// <param name="dto">
    /// The data transfer object containing the user identifier (email or phone).
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the recovery process was successfully initiated.
    /// </returns>
    Task<Result> ForgotPasswordAsync(ForgotPasswordDto dto);

    /// <summary>
    /// Verifies the validity of a password reset code.
    /// 
    /// This operation is responsible for:
    /// - Checking if the provided code is valid and has not expired.
    /// - Returning a temporary token or identifier to authorize the actual password reset.
    /// </summary>
    /// <param name="dto">
    /// The data transfer object containing the identifier and the code to check.
    /// </param>
    /// <returns>
    /// A <see cref="Result{Guid}"/> containing a validation context ID if the code is correct.
    /// </returns>
    Task<Result<Guid>> VerifyResetCodeAsync(VerifyResetCodeDto dto);

    /// <summary>
    /// Finalizes the password recovery process using a verification code.
    /// 
    /// This operation is responsible for:
    /// - Validating the recovery context.
    /// - Updating the password to the new value.
    /// - Revoking all existing sessions (security best practice).
    /// - Logging the user in immediately (returning new tokens).
    /// </summary>
    /// <param name="resetPasswordDto">
    /// The data transfer object containing the new password and verification context.
    /// </param>
    /// <returns>
    /// A <see cref="Result{AuthTokensDto}"/> containing new authentication tokens
    /// so the user does not have to log in manually after resetting.
    /// </returns>
    Task<Result<AuthTokensDto>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

    /// <summary>
    /// Validates a Two-Factor Authentication (2FA) code to complete a login attempt.
    /// 
    /// This operation is responsible for:
    /// - Verifying the time-based or SMS-based code.
    /// - Transitioning the pending login session to fully active.
    /// </summary>
    /// <param name="dto">
    /// The data transfer object containing the user context and the 2FA code.
    /// </param>
    /// <returns>
    /// A <see cref="Result{AuthTokensDto}"/> containing the Access and Refresh tokens.
    /// </returns>
    Task<Result<AuthTokensDto>> ValidateTwoFactorCodeAsync(TwoFactorVerificationDto dto);

    /// <summary>
    /// Re-sends the Two-Factor Authentication (2FA) code.
    /// 
    /// This operation is responsible for:
    /// - Handling cases where the code expired or was not received.
    /// - Enforcing rate limiting to prevent abuse.
    /// </summary>
    /// <param name="userIdentifier">
    /// The identifier of the user requesting the code resend.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the code was successfully resent.
    /// </returns>
    Task<Result> ResendTwoFactorCodeAsync(string userIdentifier);

    /// <summary>
    /// Initiates the process of changing a critical account identifier (Email/Phone).
    /// 
    /// This operation is responsible for:
    /// - Verifying that the new identifier is not already in use.
    /// - Generating a verification code sent specifically to the *new* identifier.
    /// - Creating a pending update request.
    /// </summary>
    /// <param name="dto">
    /// The data transfer object containing the new identifier details.
    /// </param>
    /// <returns>
    /// A <see cref="Result{InitiateIdentifierChangeResultDto}"/> containing reference data for the next step.
    /// </returns>
    Task<Result<InitiateIdentifierChangeResultDto>> InitiateIdentifierChangeAsync(InitiateIdentifierChangeDto dto);

    /// <summary>
    /// Finalizes the identifier change by verifying the code sent to the new identifier.
    /// 
    /// This operation is responsible for:
    /// - Validating the code against the pending update request.
    /// - Atomically updating the user's contact information.
    /// - Archiving the old identifier if necessary.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    /// <param name="dto">
    /// The data transfer object containing the verification code.
    /// </param>
    /// <returns>
    /// A <see cref="Result{AuthTokensDto}"/> containing updated tokens reflecting the new identity.
    /// </returns>
    Task<Result<AuthTokensDto>> VerifyIdentifierChangeAsync(Guid userId, VerifyIdentifierChangeDto dto);

    /// <summary>
    /// Performs an administrative request to reset a user's phone number.
    /// 
    /// This operation is responsible for:
    /// - Auditing the support action performed by an administrator.
    /// - Initiating a secure flow for the user to reclaim their account with a new number.
    /// </summary>
    /// <param name="dto">
    /// The data transfer object containing support agent ID, user ID, and new phone details.
    /// </param>
    /// <returns>
    /// A <see cref="Result{String}"/> containing the result of the support action (e.g., a magic link).
    /// </returns>
    Task<Result<string>> InitiateSupportPhoneChangeAsync(SupportResetPhoneNumberDto dto);

    /// <summary>
    /// Finalizes the phone number change initiated by support.
    /// 
    /// This operation is responsible for:
    /// - Validating the special token or code generated by the support action.
    /// - Updating the user's phone number.
    /// </summary>
    /// <param name="dto">
    /// The data transfer object containing the verification details.
    /// </param>
    /// <returns>
    /// A <see cref="Result{AuthTokensDto}"/> containing new tokens for the recovered account.
    /// </returns>
    Task<Result<AuthTokensDto>> VerifySupportPhoneChangeAsync(VerifySupportPhoneChangeDto dto);

    /// <summary>
    /// Recovers an account using a pre-generated security code.
    /// 
    /// This operation is responsible for:
    /// - Validating a backup or emergency security code.
    /// - Bypassing standard 2FA or password flows in emergency scenarios.
    /// - Resetting compromised credentials if required.
    /// </summary>
    /// <param name="dto">
    /// The data transfer object containing the security code.
    /// </param>
    /// <returns>
    /// A <see cref="Result{AuthTokensDto}"/> containing access tokens for the recovered account.
    /// </returns>
    Task<Result<AuthTokensDto>> RecoveryAccountBySecurityCodeAsync(RecoveryAccountBySecurityCodeDto dto);
}