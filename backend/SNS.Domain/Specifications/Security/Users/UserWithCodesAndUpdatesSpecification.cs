using SNS.Domain.Abstractions.Specifications;
using SNS.Domain.Security.Entities;
using System.Linq.Expressions;

namespace SNS.Domain.Specifications.Security.Users;

/// <summary>
/// Defines a specification for retrieving a single <see cref="User"/>
/// along with related verification codes and pending update data.
/// 
/// This specification is commonly used in account-related workflows
/// such as registration, activation, and verification processes
/// where additional user state information is required.
/// 
/// The user is identified using a flexible identifier that may
/// represent either an email address or a phone number.
/// </summary>
public class UserWithCodesAndUpdatesSpecification
    : ISingleEntitySpecification<User>
{
    /// <summary>
    /// Gets the criteria used to locate the user entity.
    /// 
    /// The user is matched by either email or phone number,
    /// enabling unified handling of multiple identification methods.
    /// </summary>
    public Expression<Func<User, bool>> Criteria { get; }

    /// <summary>
    /// Gets the collection of related navigation properties
    /// that should be eagerly loaded with the user entity.
    /// 
    /// This includes verification codes and pending updates
    /// required by account validation workflows.
    /// </summary>
    public List<string> Includes { get; } = new();

    /// <summary>
    /// Sorting the results in ascending order is not applicable
    /// </summary>
    public Expression<Func<User, object>>? OrderBy => null;

    /// <summary>
    /// Gets the expression used to specify the property or field by which to sort users in descending order.
    /// </summary>
    public Expression<Func<User, object>>? OrderByDescending => null;


    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="UserWithCodesAndUpdatesSpecification"/> class.
    /// </summary>
    /// <param name="identifier">
    /// A user identifier that represents either an email address
    /// or a phone number.
    /// </param>
    public UserWithCodesAndUpdatesSpecification(string identifier)
    {
        Criteria = u => u.Email == identifier || 
            u.PhoneNumber == identifier || 
            u.PendingUpdates.Any(p => p.UpdatedInfo == identifier);

        Includes.Add(nameof(User.VerificationCodes));
        Includes.Add(nameof(User.PendingUpdates));
    }
}
