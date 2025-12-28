namespace SNS.Application.Abstractions.Caching;

/// <summary>
/// Defines a contract for a distributed caching service.
/// 
/// This interface abstracts the underlying caching mechanism (e.g., Redis),
/// providing methods for key-value storage, expiration management, and
/// set-based operations for grouping related keys.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Stores an object in the cache with a specified expiration time.
    /// The object is serialized to JSON before storage.
    /// </summary>
    /// <typeparam name="T">The type of the object to cache.</typeparam>
    /// <param name="key">The unique cache key.</param>
    /// <param name="value">The object to store.</param>
    /// <param name="expiry">The duration for which the item should remain in the cache.</param>
    Task SetAsync<T>(string key, T value, TimeSpan expiry);

    /// <summary>
    /// Retrieves and deserializes an object from the cache.
    /// </summary>
    /// <typeparam name="T">The expected type of the cached object.</typeparam>
    /// <param name="key">The unique cache key.</param>
    /// <returns>
    /// The deserialized object if found; otherwise, <c>default(T)</c> (usually null).
    /// </returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Removes a specific key from the cache.
    /// </summary>
    /// <param name="key">The unique cache key to remove.</param>
    Task RemoveAsync(string key);

    // --- Set Operations (For Session Grouping) ---

    /// <summary>
    /// Adds a value to a Redis Set.
    /// Sets are useful for maintaining a list of active session IDs for a user.
    /// </summary>
    /// <param name="setKey">The key of the set (e.g., "user:sessions:{userId}").</param>
    /// <param name="value">The value to add (e.g., sessionId).</param>
    Task AddToSetAsync(string setKey, string value);

    /// <summary>
    /// Removes a specific value from a Redis Set.
    /// </summary>
    /// <param name="setKey">The key of the set.</param>
    /// <param name="value">The value to remove.</param>
    Task RemoveFromSetAsync(string setKey, string value);

    /// <summary>
    /// Retrieves all members of a Redis Set.
    /// </summary>
    /// <param name="setKey">The key of the set.</param>
    /// <returns>An array of strings containing all members of the set.</returns>
    Task<string[]> GetSetMembersAsync(string setKey);

    /// <summary>
    /// Checks if a specific value exists within a Redis Set.
    /// </summary>
    /// <param name="setKey">The key of the set.</param>
    /// <param name="value">The value to check for.</param>
    /// <returns><c>true</c> if the value exists; otherwise, <c>false</c>.</returns>
    Task<bool> IsInSetAsync(string setKey, string value);
}