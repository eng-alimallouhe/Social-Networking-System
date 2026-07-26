using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;

namespace SNS.Application.Identity.Users.Registeration.Commands.VerifyUser;

public sealed record VerifyUserCommand(
    Guid UserId, 
    string Code,
    string Token) : ICommand<AuthTokensDto>;
