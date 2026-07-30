using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Shared.DTOs.Users;

/// <summary>
/// Represents basic user account information DTO.
/// </summary>
/// <param name="Id">The unique identifier of the user account.</param>
/// <param name="UserName">The account username.</param>
/// <param name="DefaultCommunicationMethod">The default communication channel method.</param>
/// <param name="PreferredLanguage">The preferred application language setting.</param>
/// <param name="RecoveryEmail">Optional secondary recovery email address.</param>
/// <param name="Email">The primary email address.</param>
public sealed record UserBaseDto(
    Guid Id,
    string UserName,
    CommunicationMethod DefaultCommunicationMethod,
    SupportedLanguage PreferredLanguage,
    string? RecoveryEmail,
    string Email);

