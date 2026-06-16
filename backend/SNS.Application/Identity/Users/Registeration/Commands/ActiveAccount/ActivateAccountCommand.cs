using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;

namespace SNS.Application.Identity.Users.Registeration.Commands.ActiveAccount;

public sealed record ActivateAccountCommand(
    Guid UserId, 
    string Code,
    string Token) : ICommand<AuthTokensDto>;
