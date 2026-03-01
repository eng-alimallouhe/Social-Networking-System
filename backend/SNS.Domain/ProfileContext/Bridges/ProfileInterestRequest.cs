using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.ProfileContext.Bridges;

public class ProfileInterestRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid JoinerId { get; set; }
    public Guid InterestRequestId { get; set; }

    // Navigation

    public ProfileInterestRequest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}
