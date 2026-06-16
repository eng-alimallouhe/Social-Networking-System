using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;

namespace SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.ChangePassword;

public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : ICommand<AuthTokensDto>;
