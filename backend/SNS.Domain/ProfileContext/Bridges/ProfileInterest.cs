using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.ProfileContext.Bridges;

public class ProfileInterest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid InterestId { get; set; }
    public Guid ProfileId { get; set; }

    // Navigation

    public ProfileInterest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}
