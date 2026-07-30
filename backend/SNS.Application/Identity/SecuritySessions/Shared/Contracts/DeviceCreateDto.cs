using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.SecuritySessions.Shared.Contracts;

/// <summary>
/// Represents data transfer object for registering a new user device.
/// </summary>
/// <param name="UserId">The user ID associated with the device.</param>
/// <param name="DeviceToken">The unique token representing the device.</param>
/// <param name="FriendlyName">The human-readable name for the device.</param>
/// <param name="Browser">The primary browser name used on the device.</param>
/// <param name="OperatingSystem">The operating system running on the device.</param>
/// <param name="DeviceVendor">Optional manufacturer vendor name.</param>
/// <param name="DeviceModel">Optional hardware model name.</param>
/// <param name="FingerprintHash">The fingerprint hash identifying the device signature.</param>
/// <param name="IsTrusted">Flag indicating whether the device is trusted.</param>
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