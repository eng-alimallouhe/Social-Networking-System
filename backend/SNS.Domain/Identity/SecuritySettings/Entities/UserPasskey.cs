using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Identity.SecuritySettings.Entities;

public class UserPasskey : Entity, IHardDeletable
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }

    public byte[] CredentialId { get; private set; } = Array.Empty<byte>();

    public byte[] PublicKey { get; private set; } = Array.Empty<byte>();

    public string DeviceName { get; private set; } = string.Empty;

    public uint SignatureCounter { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private UserPasskey()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static UserPasskey Create(
        Guid userId,
        byte[] credentialId,
        byte[] publicKey,
        string deviceName,
        uint signatureCounter)
    {
        return new UserPasskey
        {
            UserId = userId,
            CredentialId = credentialId,
            PublicKey = publicKey,
            DeviceName = string.IsNullOrWhiteSpace(deviceName) ? "Unknown Passkey Device" : deviceName,
            SignatureCounter = signatureCounter
        };
    }

    public void UpdateCounter(uint newCounter)
    {
        if (newCounter > this.SignatureCounter)
        {
            this.SignatureCounter = newCounter;
        }
    }
}
