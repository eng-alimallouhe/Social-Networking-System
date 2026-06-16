using Microsoft.EntityFrameworkCore;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Shared.Abstractions.Specifications;
using SNS.Infrastructure.Shared.Specifications.QueryBuilders;
using System.Linq.Expressions;

namespace SNS.Infrastructure.Shared.Repositories;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class, IHardDeletable
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<TEntity?> GetSingleAsync(
        ISingleEntitySpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await QueryBuilder
            .GetQuery(_dbSet.AsQueryable(), specification)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ICollection<TEntity>>
        GetListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var items =
            await QueryBuilder
                .GetQuery(_dbSet.AsQueryable(), specification, applyPaging: true)
                .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<ICollection<TEntity>>
        GetListByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(expression)
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity?>
        GetSingleByExpressionAsync(
            Expression<Func<TEntity, bool>> expression,
            CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public async Task<bool>
        ExistsAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(expression, cancellationToken);
    }

    // ----------------------------
    // Grouping operations
    // ----------------------------

    public async Task<ICollection<TResult>>
        GetGroupedListAsync<TResult, TKey>(
            IGroupedSpecification<TEntity, TKey, TResult> specification,
            CancellationToken cancellationToken = default)
    {
        return await QueryBuilder
            .GetQuery<TEntity, TKey, TResult>(_dbSet.AsQueryable(), specification)
            .ToListAsync(cancellationToken);
    }


    // ----------------------------
    // Write operations
    // ----------------------------

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null) return;

        _dbSet.Remove(entity);
    }

    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entities);
        await Task.CompletedTask;
    }

    public async Task ExecuteDeleteAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        await _dbSet
                .Where(expression)
                .ExecuteDeleteAsync(cancellationToken);
    }
}
