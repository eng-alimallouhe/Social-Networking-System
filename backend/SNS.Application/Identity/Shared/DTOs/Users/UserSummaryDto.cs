using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Shared.DTOs.Users;

/// <summary>
/// Represents summary user account and profile overview information.
/// </summary>
public sealed record UserSummaryDto(
    Guid Id,
    string UserName,
    string? FullName,
    string Email,
    string Role,
    UserStatus Status,
    SupportedLanguage PreferredLanguage,
    CommunicationMethod DefaultCommunicationMethod,
    DateTime CreatedAt,
    string? ProfilePictureUrl
);
