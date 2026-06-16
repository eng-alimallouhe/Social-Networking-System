using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.ActivateUser;

public sealed record ActivateUserCommand(
    Guid UserId,
    string Token): ICommand<AuthTokensDto>;
