using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Shared.DTOs.Users;

/// <summary>
/// Represents user domain model capturing account state, security details, and preferences.
/// </summary>
/// <param name="UserId">The unique identifier of the user account.</param>
/// <param name="UserName">The account username.</param>
/// <param name="RoleId">The unique identifier of the assigned role.</param>
/// <param name="Email">The primary email address.</param>
/// <param name="RoleType">The system role type classification.</param>
/// <param name="RecoveryEmail">Optional secondary recovery email address.</param>
/// <param name="CommunicationMethod">The default communication method setting.</param>
/// <param name="PreferredLanguage">The preferred application language setting.</param>
/// <param name="Status">The account activity status.</param>
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

