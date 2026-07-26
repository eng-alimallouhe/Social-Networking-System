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

    public void Add(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        _dbSet.AddRange(entities);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<int> ExecuteDeleteAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(expression)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
