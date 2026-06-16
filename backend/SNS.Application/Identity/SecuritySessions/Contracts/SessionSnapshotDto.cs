namespace SNS.Application.Identity.SecuritySessions.DTOs;

public sealed record SessionSnapshotDto(
    Guid Id,
    Guid UserId,
    Guid DeviceId,
    string DeviceFingerprintHash,
    string DeviceToken,
    string Browser,
    string OS,
    string Country,
    double Longitude,
    double Latitude,
    DateTime LoginAt,
    DateTime? LogoutAt,
    DateTime LastSeenAt,
    bool IsDeviceTrusted);
