using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Shared.DTOs.Users;

public sealed record UserModel(
    Guid UserId,
    string UserName,
    Guid RoleId,
    string Email,
    RoleType RoleType,
    string? RecoveryEmail,
    CommunicationMethod CommunicationMethod,
    SupportedLanguage PreferredLanguage,
    UserStatus Status);
