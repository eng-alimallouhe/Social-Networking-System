using SNS.Domain.Abstractions.Common;
using SNS.Domain.Abstractions.Specifications;
using System.Linq.Expressions;


namespace SNS.Domain.Abstractions.Repositories;


/// <summary>
/// Represents a generic repository abstraction responsible for
/// querying and persisting entities.
/// 
/// This interface exposes read and write operations while keeping
/// the domain layer isolated from data access implementation details.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity managed by the repository.
/// </typeparam>  
public interface IRepository<TEntity> where TEntity : class, IHardDeletable
{
    // ------------------------------------------------------------------
    // Query entry point
    // ------------------------------------------------------------------

    /// <summary>
    /// Returns a filtered query based on the provided specification.
    /// 
    /// This method allows the Application layer to apply projections
    /// (e.g. using AutoMapper ProjectTo) without embedding mapping logic
    /// inside the repository itself.
    /// </summary>
    /// <param name="specification">
    /// The specification that defines filtering, sorting, pagination,
    /// and eager-loading rules.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{TEntity}"/> representing the filtered query.
    /// </returns>
    IQueryable<TEntity> GetQuery(ISpecification<TEntity> specification);

    // ------------------------------------------------------------------
    // Read operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>
    /// The entity if found; otherwise, <c>null</c>.
    /// </returns>
    Task<TEntity?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a single entity that matches the given specification.
    /// </summary>
    /// <param name="specification">
    /// The specification defining the query rules.
    /// </param>
    /// <returns>
    /// The matching entity if found; otherwise, <c>null</c>.
    /// </returns>
    Task<TEntity?> GetSingleAsync(ISingleEntitySpecification<TEntity> specification);

    /// <summary>
    /// Retrieves a collection of entities that match the given specification,
    /// along with the total count before pagination is applied.
    /// </summary>
    /// <param name="specification">
    /// The specification defining filtering, sorting, and pagination.
    /// </param>
    /// <returns>
    /// A tuple containing the resulting entities and the total record count.
    /// </returns>
    Task<(ICollection<TEntity> items, int count)>
        GetListAsync(ISpecification<TEntity> specification);

    /// <summary>
    /// Retrieves all entities that satisfy the provided expression.
    /// </summary>
    /// <param name="expression">
    /// A predicate expression used to filter entities.
    /// </param>
    Task<ICollection<TEntity>>
        GetListByExpressionAsync(Expression<Func<TEntity, bool>> expression);

    /// <summary>
    /// Retrieves a single entity that satisfies the provided expression.
    /// </summary>
    /// <param name="expression">
    /// A predicate expression used to filter the entity.
    /// </param>
    /// <param name="isTrackingEnable">
    /// Indicates whether change tracking should be enabled.
    /// </param>
    /// <returns>
    /// The matching entity if found; otherwise, <c>null</c>.
    /// </returns>
    Task<TEntity?>
        GetSingleByExpressionAsync(
            Expression<Func<TEntity, bool>> expression,
            bool isTrackingEnable = false);

    /// <summary>
    /// Returns the total number of entities that satisfy the given expression.
    /// </summary>
    /// <param name="expression">
    /// A predicate expression used to count matching entities.
    /// </param>
    Task<int>
        CountAsync(Expression<Func<TEntity, bool>> expression);

    // ------------------------------------------------------------------
    // Grouping operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Retrieves grouped and projected results based on a grouped specification.
    /// 
    /// Typically used for reporting and analytical queries.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the projected result.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of the grouping key.
    /// </typeparam>
    /// <param name="specification">
    /// The grouped specification defining grouping and projection rules.
    /// </param>
    Task<ICollection<TResult>>
        GetGroupedListAsync<TResult, TKey>(
            IGroupedSpecification<TEntity, TKey, TResult> specification);

    // ------------------------------------------------------------------
    // Write operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Adds a new entity to the data store.
    /// </summary>
    Task AddAsync(TEntity entity);

    /// <summary>
    /// Adds multiple entities to the data store.
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Deletes an entity by its unique identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the entity to delete.
    /// </param>
    Task DeleteAsync(Guid id);
}