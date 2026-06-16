using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.DTOs;

namespace SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Identifier) : ICommand<PasswordResetResponse>;
