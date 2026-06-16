using SNS.Application.Abstractions.Messaging;
using SNS.Domain.Identity.Users.Enums;

namespace SNS.Application.Identity.Users.AdminAcions.Commands.ChangeUserRole;

public sealed record ChangeUserRoleCommand(
    Guid TargetUserId,
    RoleType NewRole) : ICommand;
