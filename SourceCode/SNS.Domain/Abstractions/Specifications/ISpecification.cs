namespace SNS.Domain.Abstractions.Specifications;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

/// <summary>
/// Represents a reusable query specification that defines
/// filtering, sorting, pagination, and eager-loading rules
/// for a given entity type.
/// 
/// This interface is part of the Domain layer and contains
/// no infrastructure-specific concerns.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity to which the specification applies.
/// </typeparam>
public interface ISpecification<TEntity>
{
    /// <summary>
    /// Gets the filtering criteria that will be applied to the query.
    /// Equivalent to a WHERE clause in SQL.
    /// </summary>
    Expression<Func<TEntity, bool>> Criteria { get; }

    /// <summary>
    /// Gets a collection of navigation property paths to be eagerly loaded.
    /// These paths are interpreted by the infrastructure layer.
    /// </summary>
    List<string> Includes { get; }

    /// <summary>
    /// Indicates whether change tracking should be enabled for the query.
    /// This flag is evaluated by the data access implementation.
    /// </summary>
    bool IsTrackingEnabled { get; }

    /// <summary>
    /// Gets the expression used to order the results in ascending order.
    /// </summary>
    Expression<Func<TEntity, object>>? OrderBy { get; }

    /// <summary>
    /// Gets the expression used to order the results in descending order.
    /// </summary>
    Expression<Func<TEntity, object>>? OrderByDescending { get; }

    /// <summary>
    /// Gets the number of records to skip.
    /// Commonly used for pagination scenarios.
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Gets the maximum number of records to take.
    /// Commonly used for pagination scenarios.
    /// </summary>
    int? Take { get; }
}
