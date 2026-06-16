using SNS.Application.Identity.Shared.DTOs.SecuritySessions;
using SNS.Shared.Results;

namespace SNS.Application.Identity.SecuritySessions.Abstractions;

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
    Task<bool> ValidateAndUpdateSessionAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken);


    /// <summary>
    /// Creates a new active session for the authenticated user.
    /// 
    /// This operation is responsible for:
    /// - Generating a unique session identifier.
    /// - Capturing metadata such as device, IP, and timestamp.
    /// - Persisting the session state to the database or cache.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user logging in.
    /// </param>
    /// <returns>
    /// A <see cref="Result{Guid}"/> containing the generated Session ID
    /// if the operation completed successfully.
    /// </returns>
    Task<Result<Guid>> CreateSessionAsync(
        CreateSessionDto args, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes and removes all sessions associated with a specific user.
    /// 
    /// This operation is responsible for:
    /// - Deleting or invalidating all active and inactive session records for the user.
    /// - Forcing a "Sign Out Everywhere" scenario.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose sessions are to be cleared.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the operation
    /// completed successfully.
    /// </returns>
    Task<Result> ClearSessionsByUserIdAsync(
        Guid userId, CancellationToken cancellationToken = default);

    Task<Result> KillSwitchSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);

    Task<List<SessionRedisModel>> GetActiveSessionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

}
