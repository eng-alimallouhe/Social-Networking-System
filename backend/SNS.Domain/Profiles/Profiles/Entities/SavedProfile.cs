using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Profiles.Profiles.Entities;

public class SavedProfile : Entity, IHardDeletable
{
    //Primary Key:
    public Guid Id { get; private set; }

    //the SaverId and SavedId together should be unique to prevent duplicate saved profiles for the same saver and saved profile combination
    //Foreign Key: One(Profile) to Many(SavedProfile) as Saver Profile
    public Guid SaverId { get; private set; }

    // Foreign Key: One(Profile) to One(Profile) as Saved Profile
    public Guid SavedId { get; private set; }

    //Timestamps:
    public DateTime SavedAt { get; private set; }

    // Navigation Properties
    public Profile Saver { get; private set; } = null!;
    public Profile Saved { get; private set; } = null!;

    private SavedProfile()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        SavedAt = DateTime.UtcNow;
    }

    public static SavedProfile Create(Guid saverId, Guid savedId)
    {
        return new SavedProfile
        {
            SaverId = saverId,
            SavedId = savedId
        };
    }
}
