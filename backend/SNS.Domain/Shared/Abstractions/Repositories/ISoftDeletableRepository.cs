using SNS.Domain.Shared.Abstractions.Specifications;
using System.Linq.Expressions;


namespace SNS.Domain.Shared.Abstractions.Repositories;

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
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a single entity that matches the given specification.
    /// </summary>
    /// <param name="specification">
    /// The specification defining the query rules.
    /// </param>
    /// <returns>
    /// The matching entity if found; otherwise, <c>null</c>.
    /// </returns>
    Task<TEntity?> GetSingleAsync(ISingleEntitySpecification<TEntity> specification, CancellationToken cancellationToken);

    
    // ------------------------------------------------------------------
    // Expression Query operations
    // ------------------------------------------------------------------

    Task<ICollection<TEntity>>
        GetListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    
    Task<TEntity?>
        GetSingleByExpressionAsync(
            Expression<Func<TEntity, bool>> expression,
            CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);


    // ------------------------------------------------------------------
    // Write and delete operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Adds a new entity to the data store.
    /// </summary>
    void Add(TEntity entity);

    /// <summary>
    /// Adds multiple entities to the data store.
    /// </summary>
    void AddRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Permanently deletes an entity from the data store.
    /// 
    /// This operation performs a physical (hard) delete.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the entity to delete.
    /// </param>
    void Delete(TEntity entity);

    /// <summary>
    /// Performs a soft delete by marking the entity as deleted
    /// without physically removing it from the data store.
    /// 
    /// Typically implemented by setting an <c>IsDeleted</c> flag.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the entity to soft delete.
    /// </param>
    void SoftDelete(TEntity entity);
}

