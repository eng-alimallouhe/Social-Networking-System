namespace SNS.Application.Identity.SecuritySessions.Shared.Contracts;

public sealed record SessionCreateResult(
    Guid SessionId, 
    string RefreshToken);