using SNS.Domain.Identity.SecuritySettings.Enums;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Identity.SecuritySessions.Login.Contracts;

/// <summary>
/// Represents response DTO returned after processing a login attempt, containing tokens or MFA challenge details.
/// </summary>
/// <param name="UserId">The unique identifier of the authenticated user.</param>
/// <param name="DeviceId">The registered device identifier.</param>
/// <param name="AccessToken">The issued JWT access token if authentication is complete.</param>
/// <param name="ChallengeToken">The challenge token if 2FA or MFA is required.</param>
/// <param name="SuspendedUntil">The timestamp until which the user account is suspended, if applicable.</param>
/// <param name="SuspensionReason">The human-readable reason for account suspension, if applicable.</param>
/// <param name="RequiresTwoFactor">Indicates whether 2FA verification is required.</param>
/// <param name="IsMfaRequired">Indicates whether MFA verification is required.</param>
/// <param name="SuspensionReasonCode">The status code explaining account suspension reason.</param>
/// <param name="MfaProviderType">The configured MFA provider type required for verification.</param>
public sealed record LoginResponseDto(
    Guid? UserId = null,
    Guid? DeviceId = null,
    string? AccessToken = null,
    string? ChallengeToken = null,
    DateTime? SuspendedUntil = null,
    string? SuspensionReason = null,
    bool RequiresTwoFactor = false,
    bool IsMfaRequired = false,
    StatusCode? SuspensionReasonCode = null,
    MfaProvider? MfaProviderType = null);

