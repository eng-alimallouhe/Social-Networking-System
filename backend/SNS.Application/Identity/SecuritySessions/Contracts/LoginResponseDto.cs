using SNS.Domain.Identity.SecuritySettings.Enums;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySessions.DTOs;

public record LoginResponseDto(
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
