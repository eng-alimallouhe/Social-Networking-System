using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.DTOs;

namespace SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.ResendPasswordResetVerificationCode;

public sealed record ResendPasswordResetVerificationCodeCommand(
    string Identifier) : ICommand<PasswordResetResponse>;
