using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Shared.Abstractions.Specifications;
using System.Linq.Expressions;

namespace SNS.Domain.Identity.Users.Specifications;

/// <summary>
/// Represents a specification used to retrieve a single <see cref="User"/> entity
/// during the authentication process.
/// 
/// This specification encapsulates the query logic for resolving a user identity
/// based on different login identifier types (RecoveryEmail, Phone, or Username), ensuring
/// the correct database column is queried.
/// </summary>
public class UserWithSecuritySettingsSpecification : ISingleEntitySpecification<User>
{
    /// <summary>
    /// Gets the list of related entities to include in the query result.
    /// 
    /// Includes <see cref="User.Role"/> and <see cref="User.Profile"/> to ensure
    /// the returned user entity is fully populated for validating permissions and session creation.
    /// </summary>
    public List<string> Includes { get; }

    /// <inheritdoc/>
    public Expression<Func<User, object>>? OrderBy => null;

    /// <inheritdoc/>
    public Expression<Func<User, object>>? OrderByDescending => null;

    /// <summary>
    /// Gets the criteria expression used to filter the user.
    /// 
    /// The expression is dynamically constructed in the constructor based on the
    /// provided <see cref="IdentifierType"/>.
    /// </summary>
    public Expression<Func<User, bool>> Criteria { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserForLoginSpecification"/> class.
    /// </summary>
    /// <param name="identifier">
    /// The unique handle provided by the user (e.g., the actual email address or phone number string).
    /// </param>
    /// <param name="type">
    /// The type of the identifier, determining which property to filter against.
    /// </param>
    public UserWithSecuritySettingsSpecification(Guid UserId)
    {
        Criteria = u => u.Id == UserId;

        Includes = [nameof(User.UserSecuritySettings)];
    }
}
