namespace SNS.Application.Identity.SecuritySessions.Shared.Contracts;

/// <summary>
/// Represents the result of creating a new security session containing session ID and refresh token.
/// </summary>
/// <param name="SessionId">The unique identifier of the newly created session.</param>
/// <param name="RefreshToken">The refresh token generated for the session.</param>
public sealed record SessionCreateResult(
    Guid SessionId, 
    string RefreshToken);