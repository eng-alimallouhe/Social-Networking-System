using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Identity.SecuritySessions.Entities;

public class Device: Entity, IHardDeletable
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }

    public string DeviceToken { get; private set; } = string.Empty;
    public string? PushTarget { get; private set; }
    public string FriendlyName { get; private set; } = string.Empty;
    public string Browser { get; private set; } = string.Empty; 
    public string OperatingSystem { get; private set; } = string.Empty;

    public string? DeviceVendor { get; private set; }

    public string? DeviceModel { get; private set; }

    public string FingerprintHash { get; private set; } = string.Empty;
    public bool IsTrusted { get; private set; }

    public DateTime FirstSeenAt { get; private set; }

    public DateTime LastSeenAt { get; private set; }

    public ICollection<SecuritySession> Sessions { get; private set; } = new List<SecuritySession>();

    private Device()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        FirstSeenAt = DateTime.UtcNow;
        LastSeenAt = DateTime.UtcNow;
    }

    public static Device Create(
        Guid userId, 
        string deviceToken, 
        string friendlyName, 
        string browser, 
        string operatingSystem, 
        string? deviceVendor, 
        string? deviceModel, 
        string fingerprintHash, 
        bool isTrusted)
    {
        var entity = new Device()
        {
            UserId = userId,
            DeviceToken = deviceToken,
            FriendlyName = friendlyName,
            Browser = browser,
            OperatingSystem = operatingSystem,
            DeviceVendor = deviceVendor,
            DeviceModel = deviceModel,
            FingerprintHash = fingerprintHash,
            IsTrusted = isTrusted
        };
        return entity;
    }

    public void UpdatePushTarget(string? pushToken)
    {
        PushTarget = pushToken;
    }
}
