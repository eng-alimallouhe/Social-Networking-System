using SNS.Domain.Preferences.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Profiles.Profiles.Relations;

public class ProfileTag: Entity, IHardDeletable
{
    public Guid Id { get; private set; }
    public Guid ProfileId { get; private set; }
    public Guid TagId { get; private set; }

    public double Score { get; private set; }

    public Tag Tag { get; set; } = null!;

    public ProfileTag()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }


    public static ProfileTag Create(Guid profileId, Guid tagId, double score)
    {
        var entity = new ProfileTag();
        entity.ProfileId = profileId;
        entity.TagId = tagId;
        entity.Score = score;

        return entity;
    }


    public void IncreaseScore(double value)
    {
        Score += value;
    }
}
