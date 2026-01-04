using SNS.Domain.Abstractions.Specifications;
using SNS.Domain.Security.Entities;
using System.Linq.Expressions;

namespace SNS.Domain.Specifications.Security.Users;

/// <summary>
/// Represents a specification used to retrieve a single <see cref="User"/> entity
/// based on a hashed security code.
/// 
/// This specification encapsulates the query logic for locating a user
/// who possesses a specific security credential, typically used in 
/// account recovery or emergency access scenarios.
/// </summary>
public class UserBySecurityCodeSpecification : ISingleEntitySpecification<User>
{
    /// <summary>
    /// Gets the criteria expression used to filter the user.
    /// 
    /// Matches users where the <see cref="User.SecurityCode"/> equals the provided hash.
    /// </summary>
    public Expression<Func<User, bool>> Criteria { get; }

    /// <summary>
    /// Gets the list of related entities to include in the query result.
    /// 
    /// Includes <see cref="User.Role"/> and <see cref="User.Profile"/> to ensure
    /// the returned user entity is fully populated for security context operations.
    /// </summary>
    public List<string> Includes { get; }

    /// <inheritdoc/>
    public Expression<Func<User, object>>? OrderBy => null;

    /// <inheritdoc/>
    public Expression<Func<User, object>>? OrderByDescending => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserBySecurityCodeSpecification"/> class.
    /// </summary>
    /// <param name="codeFromUserHash">
    /// The hashed security code to search for.
    /// </param>
    public UserBySecurityCodeSpecification(string codeFromUserHash)
    {
        Criteria = u => u.SecurityCode == codeFromUserHash;
        Includes = [nameof(User.Role), nameof(User.Profile)];
    }
}