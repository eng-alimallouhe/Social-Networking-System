namespace SNS.Application.Identity.SecuritySettings.PasswordManagement.DTOs;

/// <summary>
/// Represents response DTO returned when initiating a password reset request.
/// </summary>
/// <param name="UserId">The unique identifier of the user requesting the password reset.</param>
/// <param name="Token">The verification token generated for password reset validation.</param>
public sealed record PasswordResetResponse(
    Guid UserId,
    string Token);

