using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.SecuritySessions.Shared.Contracts;

/// <summary>
/// Represents incoming client request metadata including network, location, and device details.
/// </summary>
/// <param name="IpAddress">The client IP address.</param>
/// <param name="Country">The country associated with the IP address.</param>
/// <param name="City">The city associated with the IP address.</param>
/// <param name="Latitude">The geographical latitude coordinate.</param>
/// <param name="Longitude">The geographical longitude coordinate.</param>
/// <param name="DeviceId">The client device identifier.</param>
/// <param name="Browser">The web browser user-agent summary.</param>
/// <param name="DeviceName">The user-friendly device name.</param>
/// <param name="DeviceModel">The hardware model of the device.</param>
/// <param name="DeviceVendor">The manufacturer or vendor of the device.</param>
/// <param name="FingerprintHash">The browser or device fingerprint hash.</param>
/// <param name="DeviceToken">The unique device token.</param>
/// <param name="OperatingSystem">The operating system running on the client device.</param>
public sealed record RequestInformationModel(
    string IpAddress,
    string Country,
    string City,
    double Latitude,
    double Longitude,
    Guid DeviceId,
    string Browser,
    string DeviceName,
    string DeviceModel,
    string DeviceVendor,
    string FingerprintHash,
    string DeviceToken,
    string OperatingSystem);

