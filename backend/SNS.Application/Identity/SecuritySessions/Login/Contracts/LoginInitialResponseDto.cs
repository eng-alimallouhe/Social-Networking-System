using SNS.Domain.Identity.SecuritySettings.Enums;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Identity.SecuritySessions.Login.Contracts;

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
