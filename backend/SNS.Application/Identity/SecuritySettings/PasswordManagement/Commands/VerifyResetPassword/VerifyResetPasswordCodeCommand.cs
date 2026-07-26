using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.DTOs;

namespace SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.VerifyResetPassword;

public sealed record VerifyResetPasswordCodeCommand(
    Guid UserId,
    string Token,
    string Code) : ICommand<VerifyResetPasswordResponseDto>;
