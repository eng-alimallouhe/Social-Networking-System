using SNS.Domain.Abstractions.Repositories;
using SNS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace SNS.Infrastructure.Repositories.BaseRepositories;


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

    public async Task BeginTransactionAsync()
    {
        // Avoid starting a nested transaction if one is already active
        if (_transaction != null)
        {
            return;
        }

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            // Safety fallback: ensure rollback happens on commit failure
            await RollbackTransactionAsync();
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

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
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