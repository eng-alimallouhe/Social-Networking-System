using SNS.Domain.Identity.Users.Enums;

namespace SNS.Application.Identity.Shared.DTOs.Authentication;

public sealed record AuthResponseGenerationDto(
    Guid UserId,
    Guid RoleId, 
    Guid SessionId, 
    RoleType RoleType);
