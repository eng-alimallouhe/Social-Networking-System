using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Education.Entities;

public class ProfileUniversityRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(UniversityRequest) ? Many(ProfileUniversityRequest)
    public Guid UniversityRequestId { get; set; }

    // Foreign Key: One(Profile) ? Many(ProfileUniversityRequest)
    public Guid JoinerId { get; set; }

    // Properties

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }


    // Navigation Properties

    public ProfileUniversityRequest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
