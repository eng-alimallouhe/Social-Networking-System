using SNS.Domain.Abstractions.Specifications;
using SNS.Domain.Security.Entities;
using System.Linq.Expressions;

namespace SNS.Domain.Specifications.Security.Users;

/// <summary>
/// Represents a specification used to retrieve a single <see cref="User"/> entity
/// and its associated role by the user's unique identifier.
/// 
/// This specification encapsulates the query logic for fetching the minimal
/// user data required for authorization checks (RBAC) without the overhead
/// of loading the full profile or other heavy relationships.
/// </summary>
public class UserWithRoleSpecification : ISingleEntitySpecification<User>
{
    /// <summary>
    /// Gets the criteria expression used to filter the user.
    /// 
    /// Matches the user strictly by their unique <see cref="User.Id"/>.
    /// </summary>
    public Expression<Func<User, bool>> Criteria { get; }

    /// <summary>
    /// Gets the list of related entities to include in the query result.
    /// 
    /// Includes <see cref="User.Role"/> to ensure permission validation
    /// can occur immediately after retrieval.
    /// </summary>
    public List<string> Includes { get; } = new();

    /// <inheritdoc/>
    public Expression<Func<User, object>>? OrderBy => null;

    /// <inheritdoc/>
    public Expression<Func<User, object>>? OrderByDescending => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserWithRoleSpecification"/> class.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user to retrieve.
    /// </param>
    public UserWithRoleSpecification(Guid userId)
    {
        Criteria = u => u.Id == userId;
        Includes.Add(nameof(User.Role));
    }
}