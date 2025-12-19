using SNS.Domain.Abstractions.Specifications;
using System.Linq.Expressions;


namespace SNS.Domain.Abstractions.Repositories;

/// <summary>
/// Represents a repository abstraction that supports
/// both hard deletion and soft deletion (logical delete).
/// 
/// Soft deletion means the entity is marked as deleted
/// (e.g. using an IsDeleted flag) without being physically
/// removed from the data store.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity managed by the repository.
/// </typeparam>
public interface ISoftDeletableRepository<TEntity> where TEntity : class
{
    // ------------------------------------------------------------------
    // Query entry point
    // ------------------------------------------------------------------

    /// <summary>
    /// Returns a filtered and prepared query based on the provided specification.
    /// 
    /// This method allows the Application or Service layer to apply
    /// projections (such as AutoMapper ProjectTo) while keeping
    /// data access concerns encapsulated within the repository.
    /// </summary>
    /// <param name="specification">
    /// The specification defining filtering, sorting, pagination,
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
    /// 
    /// The returned entity may be subject to soft-delete filtering
    /// depending on the repository implementation.
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
    Task<TEntity?> GetSingleAsync(ISpecification<TEntity> specification);

    /// <summary>
    /// Retrieves a collection of entities that match the given specification,
    /// along with the total count before pagination is applied.
    /// </summary>
    /// <param name="specification">
    /// The specification defining filtering, sorting, and pagination rules.
    /// </param>
    /// <returns>
    /// A tuple containing the resulting entities and the total record count.
    /// </returns>
    Task<(ICollection<TEntity> items, int count)>
        GetListAsync(ISpecification<TEntity> specification);

    /// <summary>
    /// Retrieves all entities that satisfy the provided predicate expression.
    /// </summary>
    /// <param name="expression">
    /// A predicate expression used to filter entities.
    /// </param>
    Task<ICollection<TEntity>>
        GetListByExpressionAsync(Expression<Func<TEntity, bool>> expression);

    /// <summary>
    /// Retrieves a single entity that satisfies the provided predicate expression.
    /// </summary>
    /// <param name="expression">
    /// A predicate expression used to filter the entity.
    /// </param>
    /// <param name="isTrackingEnable">
    /// Indicates whether change tracking should be enabled for the query.
    /// </param>
    /// <returns>
    /// The matching entity if found; otherwise, <c>null</c>.
    /// </returns>
    Task<TEntity?>
        GetSingleByExpressionAsync(
            Expression<Func<TEntity, bool>> expression,
            bool isTrackingEnable = false);

    /// <summary>
    /// Returns the total number of entities that satisfy the given predicate expression.
    /// </summary>
    /// <param name="expression">
    /// A predicate expression used to count matching entities.
    /// </param>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> expression);

    // ------------------------------------------------------------------
    // Grouping operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Retrieves grouped and projected results based on a grouped
    /// projection specification.
    /// 
    /// This operation is typically used for reporting and analytical
    /// read-only scenarios.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the projected result.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of the grouping key.
    /// </typeparam>
    /// <param name="specification">
    /// The grouped projection specification defining grouping,
    /// projection, and result ordering rules.
    /// </param>
    Task<ICollection<TResult>>
        GetGroupedListAsync<TResult, TKey>(
            IGroupedSpecification<TEntity, TKey, TResult> specification);

    // ------------------------------------------------------------------
    // Write and delete operations
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
    /// Permanently deletes an entity from the data store.
    /// 
    /// This operation performs a physical (hard) delete.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the entity to delete.
    /// </param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Performs a soft delete by marking the entity as deleted
    /// without physically removing it from the data store.
    /// 
    /// Typically implemented by setting an <c>IsDeleted</c> flag.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the entity to soft delete.
    /// </param>
    Task SoftDeleteAsync(Guid id);
}

