using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.CancelUserDeactivationRequest;

public sealed record CancelUserDeactivationRequestCommand(
    Guid UserId,
    string Token): ICommand<AuthTokensDto>;