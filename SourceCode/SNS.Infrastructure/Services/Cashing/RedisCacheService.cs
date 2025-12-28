using SNS.Application.Abstractions.Caching;
using StackExchange.Redis;
using System.Text.Json;

namespace SNS.Infrastructure.Services.Caching;

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
        await _db.SetAddAsync(setKey, value);
    }

    public async Task RemoveFromSetAsync(string setKey, string value)
    {
        await _db.SetRemoveAsync(setKey, value);
    }

    public async Task<string[]> GetSetMembersAsync(string setKey)
    {
        var members = await _db.SetMembersAsync(setKey);
        return members.ToStringArray()!;
    }

    public async Task<bool> IsInSetAsync(string setKey, string value)
    {
        return await _db.SetContainsAsync(setKey, value);
    }
}