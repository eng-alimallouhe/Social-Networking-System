using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;

namespace SNS.Application.Identity.SecuritySettings.EmailChange.Commands.VerifyEmailChange;

public sealed record VerifyEmailChangeCommand(
    Guid UserId, 
    string Token, 
    string Code) : ICommand<AuthTokensDto>;
