namespace SNS.Application.Identity.Shared.Abstractions;

public interface IOnlineUserTracker
{
    void Connect(Guid userId, string connectionId);
    void Disconnect(string connectionId);
    bool IsOnline(Guid userId);
}