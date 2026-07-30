using SNS.Application.Abstractions.Messaging;
using SNS.Domain.Identity.Users.Enums;

namespace SNS.Application.Identity.Users.AdminAcions.Commands.ChangeUserRole;

/// <summary>
/// Represents an administrative command to modify a user's assigned role.
/// </summary>
/// <param name="TargetUserId">The unique identifier of the user whose role is being changed.</param>
/// <param name="NewRole">The new role to assign to the target user.</param>
public sealed record ChangeUserRoleCommand(
    Guid TargetUserId,
    RoleType NewRole) : ICommand;

