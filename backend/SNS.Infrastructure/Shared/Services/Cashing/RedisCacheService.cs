using SNS.Application.Abstractions.Caching;
using StackExchange.Redis;
using System.Text.Json;

namespace SNS.Infrastructure.Shared.Services.Cashing;

/// <summary>
/// Represents the implementation of the caching service using Redis.
/// 
/// This service acts as an adapter around the StackExchange.Redis client,
/// abstracting raw Redis commands and handling object serialization/deserialization
/// to provide a strongly-typed caching interface for the application.
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
    }

    public async Task SetAsync<T>(
        string key, 
        T value, 
        TimeSpan expiry, 
        CancellationToken cancellationToken = default)
    {
        // We serialize complex objects to JSON strings because Redis 
        // primarily stores strings (or binary data) for simple keys.
        var jsonData = JsonSerializer.Serialize(value);

        await _db.StringSetAsync(key, jsonData, expiry);
    }

    public async Task<T?> GetAsync<T>(
        string key, 
        CancellationToken cancellationToken = default)
    {
        var jsonData = await _db.StringGetAsync(key);

        if (jsonData.IsNullOrEmpty) return default;

        return JsonSerializer.Deserialize<T>(jsonData.ToString());
    }

    public async Task RemoveAsync(
        string key, 
        CancellationToken cancellationToken = default)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task AddToSetAsync(
        string setKey, 
        string value, 
        CancellationToken cancellationToken = default)
    {
        // Redis Sets guarantee uniqueness, making them ideal for 
        // tracking things like "Active Users" or "Tags".
        await _db.SetAddAsync(setKey, value);
    }

    public async Task RemoveFromSetAsync(
        string setKey, 
        string value, 
        CancellationToken cancellationToken = default)
    {
        await _db.SetRemoveAsync(setKey, value);
    }

    public async Task<string[]> GetSetMembersAsync(string setKey, CancellationToken cancellationToken = default)
    {
        var members = await _db.SetMembersAsync(setKey);

        // Convert RedisValue[] to a standard string array for application consumption.
        return members.ToStringArray()!;
    }

    public async Task<bool> IsInSetAsync(string setKey, string value, CancellationToken cancellationToken = default)
    {
        return await _db.SetContainsAsync(setKey, value);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _db.KeyExistsAsync(key);
    }


    public async Task IncrementSortedSetScoreAsync(string key, string member, double increment, CancellationToken cancellationToken = default)
    {
        await _db.SortedSetIncrementAsync(key, member, increment);
    }

    public async Task TrimSortedSetAsync(string key, long startRank, long stopRank, CancellationToken cancellationToken = default)
    {
        await _db.SortedSetRemoveRangeByRankAsync(key, startRank, stopRank);
    }

    public async Task<string[]> GetTopSortedSetMembersAsync(string key, int count, CancellationToken cancellationToken = default)
    {
        var members = await _db.SortedSetRangeByRankAsync(
            key,
            start: 0,
            stop: count - 1,
            order: Order.Descending);

        return members.ToStringArray()!;
    }

    public async Task SetKeyExpiryAsync(string key, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        await _db.KeyExpireAsync(key, expiry);
    }

    public async Task<TimeSpan> GetKeyTTLAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _db.KeyTimeToLiveAsync(key) ?? TimeSpan.Zero;
    }

    public async Task<long> IncrementAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _db.StringIncrementAsync(key: key, flags: CommandFlags.None);
    }
}
