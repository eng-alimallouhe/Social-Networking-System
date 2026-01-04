using SNS.Domain.Abstractions.Specifications;
using SNS.Domain.Security.Entities;
using System.Linq.Expressions;

namespace SNS.Domain.Specifications.Security.UserSessions;

/// <summary>
/// Represents a specification used to retrieve all currently active sessions 
/// for a specific user.
/// 
/// This specification encapsulates the logic for identifying "live" sessions
/// (Active flag is true and Logout time is null), typically used for 
/// validating access, enforcing concurrency limits, or session management dashboards.
/// </summary>
public class CurrentUserSessionSpecification : ISpecification<UserSession>
{
    /// <summary>
    /// Gets the criteria expression used to filter the sessions.
    /// 
    /// Matches sessions where the User ID matches, the session is marked active,
    /// and the session has not been explicitly logged out.
    /// </summary>
    public Expression<Func<UserSession, bool>> Criteria { get; }

    /// <summary>
    /// Gets the list of related entities to include in the query result.
    /// 
    /// No related entities are eagerly loaded for this specification.
    /// </summary>
    public List<string> Includes => [];

    /// <summary>
    /// Indicates whether change tracking is enabled for the query.
    /// 
    /// Returns <c>true</c> as retrieved sessions are often modified 
    /// (e.g., updating 'LastSeenAt' or performing a soft delete/logout).
    /// </summary>
    public bool IsTrackingEnabled { get; }

    /// <inheritdoc/>
    public Expression<Func<UserSession, object>>? OrderBy => null;

    /// <inheritdoc/>
    public Expression<Func<UserSession, object>>? OrderByDescending => null;

    /// <inheritdoc/>
    public int? Skip => null;

    /// <inheritdoc/>
    public int? Take => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserSessionSpecification"/> class.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose active sessions are being retrieved.
    /// </param>
    public CurrentUserSessionSpecification(Guid userId)
    {
        Criteria = us => us.UserId == userId &&
                         us.IsActive &&
                         us.LogoutAt == null;

        IsTrackingEnabled = true;
    }
}