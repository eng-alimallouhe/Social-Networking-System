using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Users.UsersManagement.Queries.GetUserInformation;

/// <summary>
/// Represents query response DTO containing general user account information and settings.
/// </summary>
/// <param name="UserName">The account username.</param>
/// <param name="RoleName">The user's assigned system role name.</param>
/// <param name="Email">The user's primary email address.</param>
/// <param name="PreferredLanguage">The user's preferred application language.</param>
/// <param name="LastPasswordChange">The timestamp of the last password change.</param>
/// <param name="Location">The profile location.</param>
/// <param name="LastActiveLocation">The location recorded during the user's last active session.</param>
/// <param name="HasActiveDataDownloadRequest">Indicates whether a personal data export request is currently active.</param>
public sealed record UserInformationResult(
    string UserName,
    string RoleName,
    string Email,
    SupportedLanguage PreferredLanguage,
    DateTime LastPasswordChange,
    string? Location,
    string? LastActiveLocation,
    bool HasActiveDataDownloadRequest);

