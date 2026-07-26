using Microsoft.EntityFrameworkCore;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Shared.Abstractions.Specifications;
using SNS.Infrastructure.Shared.Specifications.QueryBuilders;
using System.Linq.Expressions;

namespace SNS.Infrastructure.Shared.Repositories;

public class SoftDeletableRepository<TEntity>
    : ISoftDeletableRepository<TEntity>
    where TEntity : class, ISoftDeletable
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public SoftDeletableRepository(DbContext context)
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

    public async Task<ICollection<TEntity>> GetListAsync(
        ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await QueryBuilder
                .GetQuery(_dbSet.AsQueryable(), specification, applyPaging: true)
                .ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetSingleByExpressionAsync(
            Expression<Func<TEntity, bool>> expression,
            CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(expression);
        Console.WriteLine(query.ToQueryString());
        return await _dbSet.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(expression, cancellationToken);
    }

    public async Task<ICollection<TResult>> GetGroupedListAsync<TResult, TKey>(
            IGroupedSpecification<TEntity, TKey, TResult> specification,
            CancellationToken cancellationToken = default)
    {
        return await QueryBuilder
            .GetQuery<TEntity, TKey, TResult>(_dbSet.AsQueryable(), specification)
            .ToListAsync(cancellationToken);
    }

    // ------------------------------------------------------------------
    // Write operations (تم تنظيفها من SaveChanges!)
    // ------------------------------------------------------------------

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

    public void SoftDelete(TEntity entity)
    {
        var isAlreadyDeleted = !entity.IsActive;
        if (isAlreadyDeleted)
        {
            throw new InvalidOperationException("Entity already deleted");
        }

        entity.SoftDelete();
        _context.Update(entity);
    }
}
