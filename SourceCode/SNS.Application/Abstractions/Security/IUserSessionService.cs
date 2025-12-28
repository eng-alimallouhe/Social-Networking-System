using SNS.Application.DTOs.Common;
using SNS.Application.DTOs.Security;
using SNS.Common.Results;
using SNS.Domain.Abstractions.Specifications;
using SNS.Domain.Security.Entities;

namespace SNS.Application.Abstractions.Security;

/// <summary>
/// Defines a service responsible for managing the lifecycle and retrieval 
/// of user sessions.
/// </summary>
public interface IUserSessionService
{
    /// <summary>
    /// Creates a new active session for the authenticated user and returns its ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A <see cref="Result{T}"/> containing the generated Session ID.</returns>
    Task<Result<Guid>> CreateSessionAsync(Guid userId);

    /// <summary>
    /// Retrieves a paginated list of user sessions mapped to DTOs.
    /// </summary>
    /// <param name="specification">
    /// The specification defining filtering, sorting, and pagination rules.
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the paged session summaries.
    /// </returns>
    Task<Result<Paged<UserSessionSummaryDto>>> GetSessionsAsync(ISpecification<UserSession> specification);
}