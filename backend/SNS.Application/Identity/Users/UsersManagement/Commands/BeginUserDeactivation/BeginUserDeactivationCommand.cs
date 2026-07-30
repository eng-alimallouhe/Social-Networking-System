using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.BeginUserDeactivation;

/// <summary>
/// Represents a command to initiate the user account deactivation process.
/// </summary>
/// <param name="PurgeAllContentOnHardDelete">Specifies whether user content should be permanently purged upon hard delete.</param>
public sealed record BeginUserDeactivationCommand(
    bool PurgeAllContentOnHardDelete = false) : ICommand<BeginUserDeactivationResponse>;

