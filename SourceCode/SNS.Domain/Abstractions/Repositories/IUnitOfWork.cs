namespace SNS.Domain.Abstractions.Repositories;


/// <summary>
/// Defines the contract for the Unit of Work pattern, acting as a facade
/// for managing database transactions and persistence.
/// 
/// This interface coordinates the work of multiple repositories by creating
/// a single database context shared by all of them. It ensures that all
/// data changes within a business transaction are either committed together
/// or rolled back together (Atomic Operations).
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Begins a new database transaction explicitly.
    /// 
    /// This method should be called at the start of a business process that
    /// involves modifications across multiple tables or aggregates to ensure
    /// data consistency.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation of
    /// starting the transaction.
    /// </returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits all changes made within the currently active transaction
    /// to the database permanently.
    /// 
    /// This method finalizes the transaction. If the commit fails, the
    /// transaction is automatically rolled back to maintain data integrity.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous commit operation.
    /// </returns>
    Task CommitTransactionAsync();

    /// <summary>
    /// Discards all changes made within the current transaction and restores
    /// the database state to what it was before the transaction started.
    /// 
    /// This method is typically called in a <c>catch</c> block when an
    /// exception occurs during the business process.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous rollback operation.
    /// </returns>
    Task RollbackTransactionAsync();

    /// <summary>
    /// Persists all tracked changes (inserts, updates, deletes) from the
    /// repositories to the database.
    /// 
    /// Unlike <see cref="CommitTransactionAsync"/>, this method saves changes
    /// but does not necessarily close the transaction if one is active.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> containing the number of state entries
    /// written to the database.
    /// </returns>
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}