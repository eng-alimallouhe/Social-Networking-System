using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Shared.Abstractions.Specifications;
using System.Linq.Expressions;

namespace SNS.Domain.Identity.Users.Specifications;

/// <summary>
/// Represents a specification used to retrieve a single <see cref="User"/> entity
/// and its associated role by the user's unique identifier.
/// 
/// This specification encapsulates the query logic for fetching the minimal
/// user data required for authorization checks (RBAC) without the overhead
/// of loading the full profile or other heavy relationships.
/// </summary>
public class UserWithRoleAndSettingsAndProfileSpecification : ISingleEntitySpecification<User>
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
    public List<string> Includes { get; }

    /// <inheritdoc/>
    public Expression<Func<User, object>>? OrderBy => null;

    /// <inheritdoc/>
    public Expression<Func<User, object>>? OrderByDescending => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserWithRoleAndSettingsAndProfileSpecification"/> class.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user to retrieve.
    /// </param>
    public UserWithRoleAndSettingsAndProfileSpecification(Guid userId)
    {
        Criteria = u => u.Id == userId;
        Includes = [nameof(User.Role), nameof(User.UserSecuritySettings), nameof(User.UserProfile)];
    }
}
