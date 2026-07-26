namespace SNS.Application.Identity.SecuritySessions.SessionsManagement.Contracts;

public sealed record SessionSummaryDto(
    Guid Id,
    Guid UserId,
    DateTime LoginAt,
    DateTime LastSeenAt,
    DateTime? LogoutAt,
    string City,
    string Country,
    string DeviceName,
    string Browser);
