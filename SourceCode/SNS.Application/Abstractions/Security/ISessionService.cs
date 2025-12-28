
namespace SNS.Application.Abstractions.Security;

/// <summary>
/// A lightweight service designed for high-frequency session validation.
/// It interacts primarily with the distributed cache (Redis) to verify
/// session existence and update the 'LastSeen' timestamp.
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Checks if a session exists in the cache and updates its activity timestamp.
    /// </summary>
    /// <param name="sessionId">The unique session identifier extracted from the token.</param>
    /// <returns>
    /// <c>true</c> if the session exists and was updated; 
    /// <c>false</c> if the session is missing or expired (forcing a re-login/refresh).
    /// </returns>
    Task<bool> ValidateAndUpdateSessionAsync(Guid sessionId);
}