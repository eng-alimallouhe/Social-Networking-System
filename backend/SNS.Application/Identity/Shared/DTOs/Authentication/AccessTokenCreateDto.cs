using SNS.Domain.Identity.Users.Enums;

namespace SNS.Application.Identity.Shared.DTOs.Authentication;

public sealed record AccessTokenCreateDto(
    Guid UserId,
    Guid? ProfileId,
    RoleType RoleType,
    Guid SessionId);
