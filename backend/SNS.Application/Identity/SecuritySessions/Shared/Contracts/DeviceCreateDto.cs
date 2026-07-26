using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.SecuritySessions.Shared.Contracts;

public sealed record DeviceCreateDto(
    Guid UserId,
    string DeviceToken,
    string FriendlyName,
    string Browser,
    string OperatingSystem,
    string? DeviceVendor,
    string? DeviceModel,
    string FingerprintHash,
    bool IsTrusted);