namespace SNS.Application.Identity.SecuritySessions.DTOs;

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
