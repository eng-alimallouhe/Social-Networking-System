namespace SNS.Application.Identity.SecuritySessions.DTOs;

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