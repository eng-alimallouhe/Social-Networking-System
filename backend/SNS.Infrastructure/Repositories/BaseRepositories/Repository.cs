    using global::SNS.Domain.Abstractions.Repositories;
    using global::SNS.Domain.Abstractions.Specifications;
    using Microsoft.EntityFrameworkCore;
using SNS.Domain.Abstractions.Common;
using SNS.Infrastructure.Specifications.QueryBuilders;
using System.Linq.Expressions;

    namespace SNS.Infrastructure.Repositories;

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

    public IQueryable<TEntity> GetQuery(ISpecification<TEntity> specification)
    {
        return QueryBuilder.GetQuery(_dbSet.AsQueryable(), specification);
    }

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
        var queryWithoutPaging =
            QueryBuilder.GetQuery(_dbSet.AsQueryable(), specification, applyPaging: false);

        var totalCount = await queryWithoutPaging.CountAsync();

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
        var query = isTrackingEnable ? _dbSet : _dbSet.AsNoTracking();
        return await query.FirstOrDefaultAsync(expression);
    }

    public async Task<int>
        CountAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await _dbSet.CountAsync(expression);
    }

    // ----------------------------
    // Grouping operations
    // ----------------------------

    public async Task<ICollection<TResult>>
        GetGroupedListAsync<TResult, TKey>(
            IGroupedSpecification<TEntity, TKey, TResult> specification)
    {
        return await QueryBuilder
            .GetQuery<TEntity, TKey, TResult>(_dbSet.AsQueryable(), specification)
            .ToListAsync();
    }


    // ----------------------------
    // Write operations
    // ----------------------------

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return;

        _dbSet.Remove(entity);
    }

    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }
}
