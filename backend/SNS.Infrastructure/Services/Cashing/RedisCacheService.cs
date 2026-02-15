using SNS.Application.Abstractions.Caching;
using StackExchange.Redis;
using System.Text.Json;

namespace SNS.Infrastructure.Services.Caching;

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

    public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
    {
        // We serialize complex objects to JSON strings because Redis 
        // primarily stores strings (or binary data) for simple keys.
        var jsonData = JsonSerializer.Serialize(value);

        await _db.StringSetAsync(key, jsonData, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var jsonData = await _db.StringGetAsync(key);

        if (jsonData.IsNullOrEmpty) return default;

        return JsonSerializer.Deserialize<T>(jsonData.ToString());
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task AddToSetAsync(string setKey, string value)
    {
        // Redis Sets guarantee uniqueness, making them ideal for 
        // tracking things like "Active Users" or "Tags".
        await _db.SetAddAsync(setKey, value);
    }

    public async Task RemoveFromSetAsync(string setKey, string value)
    {
        await _db.SetRemoveAsync(setKey, value);
    }

    public async Task<string[]> GetSetMembersAsync(string setKey)
    {
        var members = await _db.SetMembersAsync(setKey);

        // Convert RedisValue[] to a standard string array for application consumption.
        return members.ToStringArray()!;
    }

    public async Task<bool> IsInSetAsync(string setKey, string value)
    {
        return await _db.SetContainsAsync(setKey, value);
    }
}