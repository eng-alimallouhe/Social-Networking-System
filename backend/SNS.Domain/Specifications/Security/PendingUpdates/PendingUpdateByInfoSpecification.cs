using SNS.Domain.Abstractions.Specifications;
using SNS.Domain.Security.Entities;
using System.Linq.Expressions;

namespace SNS.Domain.Specifications.Security.PendingUpdates;


/// <summary>
/// Defines a specification for retrieving a single
/// <see cref="PendingUpdate"/> entity based on its updated value.
/// 
/// This specification is used to verify pending user changes
/// such as email or phone number updates before confirmation.
/// </summary>
public class PendingUpdateByInfoSpecification
    : ISingleEntitySpecification<PendingUpdate>
{
    /// <summary>
    /// Gets the criteria used to locate the pending update entity.
    /// </summary>
    public Expression<Func<PendingUpdate, bool>> Criteria { get; }

    /// <summary>
    /// Gets the collection of related navigation properties
    /// to be eagerly loaded.
    /// </summary>
    public List<string> Includes { get; } = new();


    /// <summary>
    /// Sorting the results in ascending order by the specified key.
    /// This is not used in this specification.
    /// </summary>
    public Expression<Func<PendingUpdate, object>>? OrderBy => null;


    /// <summary>
    /// Sorting the results in descending order by RequestedAt Property.
    /// </summary>
    public Expression<Func<PendingUpdate, object>>? OrderByDescending => p => p.RequestedAt;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="PendingUpdateByInfoSpecification"/> class.
    /// </summary>
    /// <param name="updatedInfo">
    /// The new value awaiting confirmation.
    /// </param>
    public PendingUpdateByInfoSpecification(string updatedInfo)
    {
        Criteria = p => p.UpdatedInfo == updatedInfo;
    }
}
