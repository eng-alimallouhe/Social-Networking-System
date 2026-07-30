namespace SNS.Application.Identity.SecuritySettings.PasswordManagement.DTOs;

/// <summary>
/// Represents response DTO returned after successfully verifying a password reset code.
/// </summary>
/// <param name="UserId">The unique identifier of the user resetting the password.</param>
/// <param name="Token">The confirmation token required for the final password reset step.</param>
public sealed record VerifyResetPasswordResponseDto(
    Guid UserId,
    string Token);

