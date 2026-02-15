using SNS.Domain.Abstractions.Specifications;
using SNS.Domain.Security.Entities;
using System.Linq.Expressions;

namespace SNS.Domain.Specifications.Security.Users;

/// <summary>
/// Represents a specification used to retrieve a single <see cref="User"/> entity
/// by its unique identifier, including essential role and profile associations.
/// 
/// This specification encapsulates the query logic for fetching the core
/// user aggregate root necessary for authorization checks and profile-related operations.
/// </summary>
public class UserWithProfileAndRoleSpecification : ISingleEntitySpecification<User>
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
    /// Includes <see cref="User.Profile"/> and <see cref="User.Role"/> to ensure
    /// the returned entity has all necessary context for domain logic.
    /// </summary>
    public List<string> Includes { get; } = new();

    /// <inheritdoc/>
    public Expression<Func<User, object>>? OrderBy => null;

    /// <inheritdoc/>
    public Expression<Func<User, object>>? OrderByDescending => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserWithProfileAndRoleSpecification"/> class.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user to retrieve.
    /// </param>
    public UserWithProfileAndRoleSpecification(Guid userId)
    {
        Criteria = u => u.Id == userId;
        Includes.Add(nameof(User.Profile));
        Includes.Add(nameof(User.Role));
    }
}