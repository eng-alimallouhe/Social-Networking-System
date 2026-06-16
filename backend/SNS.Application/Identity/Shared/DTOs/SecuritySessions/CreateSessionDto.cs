namespace SNS.Application.Identity.Shared.DTOs.SecuritySessions;

public sealed record CreateSessionDto(
    Guid UserId,
    Guid DeviceId,
    string IpAddress,
    string City,
    string Country,
    string FingerprintHash,
    string Browser,
    double Longitude,
    double Latitude,
    bool IsDeviceTrusted);
