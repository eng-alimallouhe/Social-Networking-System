namespace SNS.Domain.Abstractions.Specifications;

using System;
using System.Linq.Expressions;

/// <summary>
/// Represents a specification that performs grouping and aggregation
/// over a set of entities and produces a projected result.
/// 
/// This interface is typically used for reporting, analytics,
/// or read-only query scenarios.
/// </summary>
/// <typeparam name="TEntity">
/// The entity type being queried.
/// </typeparam>
/// <typeparam name="TKey">
/// The type of the grouping key.
/// </typeparam>
/// <typeparam name="TResult">
/// The type of the projected result after grouping.
/// </typeparam>
public interface IGroupedSpecification<TEntity, TKey, TResult> : ISpecification<TEntity>
{
    /// <summary>
    /// Gets the expression that defines how entities are grouped.
    /// Equivalent to a GROUP BY clause in SQL.
    /// </summary>
    Expression<Func<TEntity, TKey>> GroupBy { get; }

    /// <summary>
    /// Gets the expression that defines how grouped entities
    /// are transformed into the final result.
    /// </summary>
    Expression<Func<IGrouping<TKey, TEntity>, TResult>> Selector { get; }

    /// <summary>
    /// Gets an optional ordering function applied to the final
    /// grouped and projected results.
    /// </summary>
    Func<IQueryable<TResult>, IOrderedQueryable<TResult>>? ResultOrdering { get; }
}
