using SNS.Domain.Abstractions.Specifications;
using SNS.Domain.Security;
using SNS.Domain.Security.Entities;
using System.Linq.Expressions;

namespace SNS.Domain.Specifications.Security.VerificationCodes;


/// <summary>
/// Defines a specification for retrieving an active
/// <see cref="VerificationCode"/> associated with a user.
/// 
/// This specification is used to validate verification codes
/// such as account activation or security confirmation codes,
/// ensuring that the code is still active and unused.
/// </summary>
public class ActiveVerificationCodeSpecification
    : ISingleEntitySpecification<VerificationCode>
{
    /// <summary>
    /// Gets the criteria used to locate the verification code.
    /// </summary>
    public Expression<Func<VerificationCode, bool>> Criteria { get; }

    /// <summary>
    /// Gets the collection of related navigation properties
    /// to be eagerly loaded.
    /// </summary>
    public List<string> Includes { get; } = new();

    /// <summary>
    /// Gets the expression used to specify the property or properties by which to order the collection of verification
    /// codes.
    /// </summary>
    /// <remarks>
    /// Use this property to define the ordering criteria when querying or sorting verification
    /// codes. The expression typically represents a property of the VerificationCode entity to be used for ordering
    /// results.
    /// </remarks>
    public Expression<Func<VerificationCode, object>>? OrderBy => null;

    /// <summary>
    /// Gets the expression used to order verification codes by creation date in descending order.
    /// <remarks>
    /// Use this property to sort verification codes so that the most recently created codes appear first.
    /// </remarks>
    /// </summary>
    public Expression<Func<VerificationCode, object>>? OrderByDescending => c => c.CreatedAt;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ActiveVerificationCodeSpecification"/> class
    /// using a specific verification code value.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user owning the verification code.
    /// </param>
    /// <param name="type">
    /// The type of verification code.
    /// </param>
    /// <param name="code">
    /// The verification code value to be validated.
    /// </param>
    public ActiveVerificationCodeSpecification(
        Guid userId,
        CodeType type,
        string code)
    {
        Criteria = c =>
            c.UserId == userId &&
            c.Type == type &&
            c.Code == code &&
            !c.IsUsed;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ActiveVerificationCodeSpecification"/> class
    /// without matching the code value.
    /// 
    /// This overload is useful for scenarios such as validating
    /// attempt limits or checking for existing active codes.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user owning the verification code.
    /// </param>
    /// <param name="type">
    /// The type of verification code.
    /// </param>
    public ActiveVerificationCodeSpecification(
        Guid userId,
        CodeType type)
    {
        Criteria = c =>
            c.UserId == userId &&
            c.Type == type &&
            !c.IsUsed;
    }
}
