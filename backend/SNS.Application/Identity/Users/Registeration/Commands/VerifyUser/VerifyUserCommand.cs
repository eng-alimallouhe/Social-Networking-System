using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;

namespace SNS.Application.Identity.Users.Registeration.Commands.VerifyUser;

/// <summary>
/// Represents a command to verify a user's account using an activation verification code and token.
/// </summary>
/// <param name="UserId">The unique identifier of the user account being verified.</param>
/// <param name="Code">The numeric verification code sent to the user.</param>
/// <param name="Token">The activation token associated with the verification request.</param>
public sealed record VerifyUserCommand(
    Guid UserId, 
    string Code,
    string Token) : ICommand<AuthTokensDto>;

