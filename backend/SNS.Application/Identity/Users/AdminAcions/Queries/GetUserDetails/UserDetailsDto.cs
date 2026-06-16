using SNS.Application.Identity.SecuritySessions.Contracts;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Users.AdminAcions.Queries.GetUserDetails;

public sealed record UserDetailsDto(
    Guid UserId,
    string UserName,
    string Email,
    UserStatus Status,
    bool IsVerified,
    int FailedLoginAttempts,
    SupportedLanguage PreferredLanguage,

    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime LastLogIn,
    DateTime LastPasswordChange,

    DateTime? SuspendedUntil,
    string? SuspensionReason,
    DateTime? DeactivatedAt,

    ProfileBaseDto Profile,
    UserRoleDetailsDto Role,
    UserMetricsDto Metrics,
    IReadOnlyCollection<SessionSummaryDto> ActiveSessions,
    IReadOnlyCollection<SessionSummaryDto> UserSessions
);



public sealed record UserRoleDetailsDto(
    Guid RoleId,
    RoleType RoleType
);

public sealed record UserMetricsDto(
    int TotalActiveSessions,
    int TotalRegisteredDevices,
    int TotalPasskeys,
    int TotalIdentityArchives, 
    int TotalPasswordArchives  
);