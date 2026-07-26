using SNS.Application.Identity.Shared.Abstractions;
using System.Collections.Concurrent;

namespace SNS.Application.Identity.Shared.Services;

internal class OnlineUserTracker
    : IOnlineUserTracker
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, byte>> _userConnections = new();

    private readonly ConcurrentDictionary<string, Guid> _connectionUsers = new();

    public void Connect(Guid userId, string connectionId)
    {
        var connections = _userConnections.GetOrAdd(
            userId,
            _ => new ConcurrentDictionary<string, byte>());

        connections.TryAdd(connectionId, 0);

        _connectionUsers.TryAdd(connectionId, userId);
    }

    public void Disconnect(string connectionId)
    {
        if (!_connectionUsers.TryRemove(connectionId, out var userId))
            return;

        if (!_userConnections.TryGetValue(userId, out var connections))
            return;

        connections.TryRemove(connectionId, out _);

        if (connections.IsEmpty)
        {
            _userConnections.TryRemove(userId, out _);
        }
    }

    public bool IsOnline(Guid userId)
    {
        return _userConnections.ContainsKey(userId);
    }
}
