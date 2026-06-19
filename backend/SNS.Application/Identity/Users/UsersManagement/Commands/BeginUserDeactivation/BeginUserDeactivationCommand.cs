using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.BeginUserDeactivation;

public sealed record BeginUserDeactivationCommand(
    bool PurgeAllContentOnHardDelete = false) : ICommand<BeginUserDeactivationResponse>;
