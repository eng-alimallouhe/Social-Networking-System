using SNS.Domain.Identity.Shared.Enums;

namespace SNS.Application.Identity.Users.UsersManagement.Queries.GetUserSecurityDetails;

/// <summary>
/// Represents query response DTO containing user security settings and recovery options status.
/// </summary>
/// <param name="IsMfaEnabled">Flag indicating whether multi-factor authentication is enabled.</param>
/// <param name="MfaProvider">The configured MFA provider name.</param>
/// <param name="IsAuthenticatorAppLinked">Flag indicating whether an authenticator app is linked.</param>
/// <param name="PasskeysCount">The total count of registered WebAuthn passkeys.</param>
/// <param name="LastPasswordChange">The timestamp when the user password was last changed.</param>
/// <param name="TotalDevicesCount">The total count of registered devices for the user.</param>
/// <param name="RecoveryEmail">Optional recovery email address associated with the user.</param>
/// <param name="UsedRecoveryCodesCount">The count of used recovery codes.</param>
/// <param name="UnusedRecoveryCodesCount">The count of remaining active recovery codes.</param>
public sealed record UserSecurityDetailsResult(
    bool IsMfaEnabled,
    string MfaProvider,
    bool IsAuthenticatorAppLinked,
    CommunicationMethod DefualtCommunicationMethod,
    int PasskeysCount,
    DateTime LastPasswordChange,
    int TotalDevicesCount,
    string? RecoveryEmail,
    int UsedRecoveryCodesCount,
    int UnusedRecoveryCodesCount);

