using SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetUserSessions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Users.AdminAcions.Queries.GetUserDetails;

/// <summary>
/// Represents comprehensive user account details for administrative inspection.
/// </summary>
/// <param name="UserId">The unique identifier of the user account.</param>
/// <param name="UserName">The account username.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="Status">The account status (Active, Banned, Suspended, etc.).</param>
/// <param name="IsVerified">Flag indicating email verification status.</param>
/// <param name="FailedLoginAttempts">Number of consecutive failed login attempts.</param>
/// <param name="PreferredLanguage">The user's preferred application language.</param>
/// <param name="CreatedAt">The account registration timestamp.</param>
/// <param name="UpdatedAt">The last update timestamp.</param>
/// <param name="LastLogIn">The last successful login timestamp.</param>
/// <param name="LastPasswordChange">The timestamp of the last password change.</param>
/// <param name="SuspendedUntil">Optional timestamp indicating when suspension ends.</param>
/// <param name="SuspensionReason">Optional reason for account suspension.</param>
/// <param name="DeactivatedAt">Optional deactivation timestamp.</param>
/// <param name="Profile">The associated user profile summary.</param>
/// <param name="Role">The user's system role details.</param>
/// <param name="Metrics">User metric counts for sessions, devices, and archives.</param>
/// <param name="ActiveSessions">The collection of active security sessions.</param>
/// <param name="UserSessions">The collection of historical user security sessions.</param>
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

/// <summary>
/// Represents user system role details.
/// </summary>
/// <param name="RoleId">The unique identifier of the assigned role.</param>
/// <param name="RoleType">The system role classification.</param>
public sealed record UserRoleDetailsDto(
    Guid RoleId,
    RoleType RoleType
);

/// <summary>
/// Represents aggregated user security and account activity metric counters.
/// </summary>
/// <param name="TotalActiveSessions">Total count of active security sessions.</param>
/// <param name="TotalRegisteredDevices">Total count of registered devices.</param>
/// <param name="TotalPasskeys">Total count of registered WebAuthn passkeys.</param>
/// <param name="TotalIdentityArchives">Total count of identity archive records.</param>
/// <param name="TotalPasswordArchives">Total count of historical password records.</param>
public sealed record UserMetricsDto(
    int TotalActiveSessions,
    int TotalRegisteredDevices,
    int TotalPasskeys,
    int TotalIdentityArchives, 
    int TotalPasswordArchives  
);