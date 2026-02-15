using SNS.Domain.Abstractions.Specifications;
using SNS.Domain.Security.Entities;
using SNS.Domain.Security.Enums;
using System.Linq.Expressions;

namespace SNS.Domain.Specifications.Security.Users;

/// <summary>
/// Represents a specification used to retrieve a single <see cref="User"/> entity
/// during the authentication process.
/// 
/// This specification encapsulates the query logic for resolving a user identity
/// based on different login identifier types (Email, Phone, or Username), ensuring
/// the correct database column is queried.
/// </summary>
public class UserForLoginSpecification : ISingleEntitySpecification<User>
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
    public UserForLoginSpecification(string identifier, IdentifierType type)
    {
        switch (type)
        {
            case IdentifierType.Email:
                Criteria = u => u.Email == identifier;
                break;
            case IdentifierType.Phone:
                Criteria = u => u.PhoneNumber == identifier;
                break;
            default:
                Criteria = u => u.UserName == identifier;
                break;
        }

        Includes = new List<string> { "Role", "Profile" };
    }
}