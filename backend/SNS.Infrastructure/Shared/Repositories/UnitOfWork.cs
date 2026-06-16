using Microsoft.EntityFrameworkCore.Storage;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Persistence;

namespace SNS.Infrastructure.Shared.Repositories;


/// <summary>
/// Implementation of the <see cref="IUnitOfWork"/> interface using Entity Framework Core.
/// 
/// This class manages the lifecycle of the <see cref="SNSDbContext"/> and wraps
/// the EF Core transaction APIs to provide a simplified transaction management flow.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly SNSDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(SNSDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // Avoid starting a nested transaction if one is already active
        if (_transaction != null)
        {
            return;
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            // Safety fallback: ensure rollback happens on commit failure
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        // Dispose the context and any active transaction to prevent memory leaks
        _context.Dispose();
        _transaction?.Dispose();
        GC.SuppressFinalize(this);
    }
}
