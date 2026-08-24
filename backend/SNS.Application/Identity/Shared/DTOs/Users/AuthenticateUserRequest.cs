using SNS.Domain.Identity.Users.Enums;

namespace SNS.Application.Identity.Shared.DTOs.Users;

public record AuthenticateUserRequest(
    Guid UserId,
    Guid RoleId,
    RoleType RoleType,
    Guid? ProfileId,
    Guid? SessionId
);