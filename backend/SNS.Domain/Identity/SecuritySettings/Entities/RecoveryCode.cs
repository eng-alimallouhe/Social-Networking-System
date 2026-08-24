using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Identity.SecuritySettings.Entities;

public class RecoveryCode : Entity, IHardDeletable
{
    // Primary Key:
    public Guid Id { get; private set; }

    //Foreign Key: One(UserSecuritySettings) to Many(RecoveryCodes)
    public Guid UserSecuritySettingsId { get; private set; }

    //Must be a hash of BCrypt algorithm with a cost factor of 12 or higher, and must be unique across all RecoveryCodes in the database.
    //Unique Filed: 
    public string CodeHash { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime? UsedAt { get; private set; } = null;

    private RecoveryCode()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        IsUsed = false;
    }

    public static RecoveryCode Create(Guid userSecuritySettingsId, string codeHash)
    {
        var entity = new RecoveryCode()
        {
            CodeHash = codeHash
        };
        entity.UserSecuritySettingsId = userSecuritySettingsId;
        
        return entity;
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
    }
}
