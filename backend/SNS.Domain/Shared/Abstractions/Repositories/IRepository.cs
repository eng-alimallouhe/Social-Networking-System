using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Abstractions.Specifications;
using System.Linq.Expressions;


namespace SNS.Domain.Shared.Abstractions.Repositories;


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
    // Read operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Retrieves an entity by its unique identifier.
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

    Task<ICollection<TEntity>> GetListByExpressionAsync(
        Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

    // ------------------------------------------------------------------
    // Write operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Adds a new entity to the data store.
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Adds multiple entities to the data store.
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an entity by its unique identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the entity to delete.
    /// </param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes List of entities.
    /// </summary>
    /// <param name="entities">
    /// The list that contains entities to delete.
    /// </param>

    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    Task ExecuteDeleteAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
}