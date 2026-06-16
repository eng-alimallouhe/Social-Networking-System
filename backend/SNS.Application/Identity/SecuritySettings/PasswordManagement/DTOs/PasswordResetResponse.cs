namespace SNS.Application.Identity.SecuritySettings.PasswordManagement.DTOs;

public sealed record PasswordResetResponse(
    Guid UserId,
    string Token);
