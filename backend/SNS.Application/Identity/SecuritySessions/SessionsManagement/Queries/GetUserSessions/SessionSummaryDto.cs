namespace SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetUserSessions;

public sealed record SessionSummaryDto(
    Guid UserId,
    Guid Id,
    string DeviceName,
    DateTime LoginAt,
    DateTime LastSeenAt,
    DateTime? LogoutAt,
    string Counrty,
    string City,
    int DurationMinutes,
    bool IsRevoked,
    string? RevokedReason);
