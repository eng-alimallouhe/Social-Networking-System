namespace SNS.Application.Identity.Users.UsersManagement.Commands.BeginUserDeactivation;

public sealed record BeginUserDeactivationResponse(
    Guid UserId,
    string Token);
