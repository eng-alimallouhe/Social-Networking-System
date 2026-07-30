namespace SNS.Application.Identity.Users.UsersManagement.Commands.BeginUserDeactivation;

/// <summary>
/// Represents response DTO returned when initiating user account deactivation, containing user ID and confirmation token.
/// </summary>
/// <param name="UserId">The unique identifier of the user requesting deactivation.</param>
/// <param name="Token">The verification token required to complete account deactivation.</param>
public sealed record BeginUserDeactivationResponse(
    Guid UserId,
    string Token);

