using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.CancelUserDeactivationRequest;

/// <summary>
/// Represents a command to cancel an active account deactivation request and reactivate the user account.
/// </summary>
/// <param name="UserId">The unique identifier of the user cancelling deactivation.</param>
/// <param name="Token">The verification token issued for the deactivation cancellation challenge.</param>
public sealed record CancelUserDeactivationRequestCommand(
    Guid UserId,
    string Token): ICommand<AuthTokensDto>;