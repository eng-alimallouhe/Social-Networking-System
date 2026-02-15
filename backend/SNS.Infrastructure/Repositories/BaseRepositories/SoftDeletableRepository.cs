using Microsoft.EntityFrameworkCore;
using SNS.Domain.Abstractions.Common;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Abstractions.Specifications;
using SNS.Infrastructure.Specifications.QueryBuilders;
using System.Linq.Expressions;

namespace SNS.Infrastructure.Repositories;

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

    // ------------------------------------------------------------------
    // Query entry point (final implementation)
    // ------------------------------------------------------------------

    public IQueryable<TEntity> GetQuery(ISpecification<TEntity> specification)
    {
        return QueryBuilder.GetQuery(_dbSet.AsQueryable(), specification);
    }

    // ------------------------------------------------------------------
    // Read operations (final implementation)
    // ------------------------------------------------------------------

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<TEntity?> GetSingleAsync(
        ISingleEntitySpecification<TEntity> specification)
    {
        return await QueryBuilder
            .GetQuery(_dbSet.AsQueryable(), specification)
            .FirstOrDefaultAsync();
    }

    public async Task<(ICollection<TEntity> items, int count)>
        GetListAsync(ISpecification<TEntity> specification)
    {
        var countQuery =
            QueryBuilder.GetQuery(_dbSet.AsQueryable(), specification, applyPaging: false);

        var totalCount = await countQuery.CountAsync();

        var items =
            await QueryBuilder
                .GetQuery(_dbSet.AsQueryable(), specification, applyPaging: true)
                .ToListAsync();

        return (items, totalCount);
    }

    public async Task<ICollection<TEntity>>
        GetListByExpressionAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await _dbSet
            .Where(expression)
            .ToListAsync();
    }

    public async Task<TEntity?>
        GetSingleByExpressionAsync(
            Expression<Func<TEntity, bool>> expression,
            bool isTrackingEnable = false)
    {
        var query = isTrackingEnable
            ? _dbSet.AsQueryable()
            : _dbSet.AsNoTracking();

        return await query.FirstOrDefaultAsync(expression);
    }

    public async Task<int>
        CountAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await _dbSet.CountAsync(expression);
    }

    // ------------------------------------------------------------------
    // Grouping operations (final implementation)
    // ------------------------------------------------------------------

    public async Task<ICollection<TResult>>
        GetGroupedListAsync<TResult, TKey>(
            IGroupedSpecification<TEntity, TKey, TResult> specification)
    {
        return await QueryBuilder
            .GetQuery<TEntity, TKey, TResult>(_dbSet.AsQueryable(), specification)
            .ToListAsync();
    }


    // ------------------------------------------------------------------
    // Write operations (final implementation)
    // ------------------------------------------------------------------

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) return;

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    // ------------------------------------------------------------------
    // Soft delete (customizable)
    // ------------------------------------------------------------------

    /// <summary>
    /// Performs a logical (soft) delete.
    /// 
    /// The default implementation assumes the entity
    /// has an <c>IsDeleted</c> property.
    /// 
    /// Override this method only if the entity requires
    /// a different soft delete behavior.
    /// </summary>
    public async Task SoftDeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) return;

        // default convention-based soft delete
        var isDeletedProperty = !entity.IsActive;
        if (isDeletedProperty)
            throw new InvalidOperationException(
                $"Entity already deleted");

        entity.IsActive = false;
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }

    
}
