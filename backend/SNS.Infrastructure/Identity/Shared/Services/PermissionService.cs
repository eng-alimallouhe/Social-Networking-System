using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SNS.Application.Abstractions.Caching;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;

namespace SNS.Infrastructure.Identity.Shared.Services;

/// <summary>
/// Distributed Redis-cached permission service for evaluating role-permission grants using IApplicationDbContext for queries.
/// </summary>
public class PermissionService : IPermissionService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(2);

    private readonly ICacheService _cacheService;
    private readonly IIdentityCacheKeyFactory _identityCacheKeyFactory;
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(
        ICacheService cacheService,
        IIdentityCacheKeyFactory identityCacheKeyFactory,
        IApplicationDbContext dbContext,
        ILogger<PermissionService> logger)
    {
        _cacheService = cacheService;
        _identityCacheKeyFactory = identityCacheKeyFactory;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> HasPermissionAsync(string roleName, string permission, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(permission))
        {
            return false;
        }

        var matrix = await GetRolePermissionsMatrixAsync(cancellationToken);
        if (matrix.TryGetValue(roleName, out var permissions))
        {
            return permissions.Contains(permission);
        }

        return false;
    }

    public async Task<IReadOnlySet<string>> GetPermissionsForRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return new HashSet<string>();
        }

        var matrix = await GetRolePermissionsMatrixAsync(cancellationToken);
        if (matrix.TryGetValue(roleName, out var permissions))
        {
            return permissions;
        }

        return new HashSet<string>();
    }

    public async Task InvalidateCacheAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = _identityCacheKeyFactory.GetRolePermissionsMatrixKey();
        try
        {
            await _cacheService.RemoveAsync(cacheKey, cancellationToken);
            _logger.LogInformation("Role permissions cache invalidated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to invalidate role permissions cache in Redis.");
        }
    }

    private async Task<Dictionary<string, HashSet<string>>> GetRolePermissionsMatrixAsync(CancellationToken cancellationToken)
    {
        var cacheKey = _identityCacheKeyFactory.GetRolePermissionsMatrixKey();

        // 1. Attempt to read from Redis distributed cache
        try
        {
            var cachedMatrix = await _cacheService.GetAsync<Dictionary<string, HashSet<string>>>(cacheKey, cancellationToken);
            if (cachedMatrix != null && cachedMatrix.Count > 0)
            {
                // Ensure case-insensitive comparison
                return new Dictionary<string, HashSet<string>>(cachedMatrix, StringComparer.OrdinalIgnoreCase);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve role permissions matrix from Redis cache. Falling back to database.");
        }

        // 2. Fallback to database on cache miss or Redis error using IApplicationDbContext read-only query
        try
        {
            var rolesWithPermissions = await _dbContext.Roles
                .AsNoTracking()
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ToListAsync(cancellationToken);

            var matrix = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var role in rolesWithPermissions)
            {
                var roleKey = role.Type.ToString();
                var permissions = role.RolePermissions
                    .Where(rp => rp.Permission != null && !string.IsNullOrWhiteSpace(rp.Permission.Name))
                    .Select(rp => rp.Permission.Name)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                matrix[roleKey] = permissions;
            }

            // 3. Populate Redis distributed cache
            try
            {
                await _cacheService.SetAsync(cacheKey, matrix, CacheDuration, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to populate role permissions matrix in Redis cache.");
            }

            return matrix;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load role permissions matrix from database. Failing closed.");
            // Fail closed: return empty matrix so authorization is denied rather than granted
            return new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
