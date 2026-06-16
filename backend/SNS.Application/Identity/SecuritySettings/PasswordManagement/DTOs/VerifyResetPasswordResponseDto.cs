namespace SNS.Application.Identity.SecuritySettings.PasswordManagement.DTOs;

public sealed record VerifyResetPasswordResponseDto(
    Guid UserId,
    string Token);
