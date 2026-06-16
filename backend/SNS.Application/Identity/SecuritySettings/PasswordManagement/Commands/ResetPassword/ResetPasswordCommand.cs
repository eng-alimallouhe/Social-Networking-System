using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;

namespace SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.ResetPassword;

public sealed record ResetPasswordCommand(Guid UserId, string Token, string NewPassword) : ICommand<AuthTokensDto>;
