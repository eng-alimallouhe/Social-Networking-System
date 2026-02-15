using SNS.Application.DTOs.Common;
using SNS.Application.DTOs.Security;
using SNS.Common.Results;
using SNS.Domain.Abstractions.Specifications;
using SNS.Domain.Security.Entities;

namespace SNS.Application.Abstractions.Security;

/// <summary>
/// Represents a domain service responsible for
/// managing the lifecycle and retrieval of user sessions.
/// 
/// This service encapsulates the business logic related to
/// session creation, historical tracking, and mass revocation (clearing sessions),
/// while keeping the Application layer decoupled from infrastructure and implementation details.
/// </summary>
public interface IUserSessionService
{
    // ------------------------------------------------------------------
    // Command operations
    // ------------------------------------------------------------------

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
        Guid userId);

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
        Guid userId);

    // ------------------------------------------------------------------
    // Query operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Retrieves a paginated list of user sessions based on the provided criteria.
    /// 
    /// This method does not mutate state and is intended for
    /// displaying session history or active device lists to the user.
    /// </summary>
    /// <param name="specification">
    /// The specification defining filtering, sorting, and pagination rules
    /// (e.g., filter by IsActive, sort by LastSeenAt).
    /// </param>
    /// <returns>
    /// A <see cref="Result{Paged{UserSessionSummaryDto}}"/> containing the paged session summaries
    /// if found; otherwise, an appropriate failure result.
    /// </returns>
    Task<Result<Paged<UserSessionSummaryDto>>> GetSessionsAsync(
        ISpecification<UserSession> specification);
}