using SNS.Domain.Identity.SecuritySettings.Enums;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Identity.SecuritySessions.Login.Contracts;

/// <summary>
/// Represents response DTO returned during the initial login step, containing authentication tokens or MFA challenge details.
/// </summary>
/// <param name="UserId">The unique identifier of the user logging in.</param>
/// <param name="DeviceId">The registered device identifier.</param>
/// <param name="AccessToken">The issued JWT access token if authentication is complete.</param>
/// <param name="RefreshToken">The issued refresh token if authentication is complete.</param>
/// <param name="ChallengeToken">The challenge token if two-factor authentication or MFA is required.</param>
/// <param name="SuspendedUntil">The timestamp until which the user account is suspended, if applicable.</param>
/// <param name="SuspensionReason">The human-readable reason for account suspension, if applicable.</param>
/// <param name="RequiresTwoFactor">Indicates whether 2FA verification is required to complete login.</param>
/// <param name="IsMfaRequired">Indicates whether MFA verification is required to complete login.</param>
/// <param name="SuspensionReasonCode">The status code explaining account suspension reason.</param>
/// <param name="MfaProviderType">The configured MFA provider type required for verification.</param>
public record LoginInitialResponseDto(
    Guid? UserId = null,
    Guid? DeviceId = null,
    string? AccessToken = null,
    string? RefreshToken = null,
    string? ChallengeToken = null,
    DateTime? SuspendedUntil = null,
    string? SuspensionReason = null,
    bool RequiresTwoFactor = false,
    bool IsMfaRequired = false,
    StatusCode? SuspensionReasonCode = null,
    MfaProvider? MfaProviderType = null);

