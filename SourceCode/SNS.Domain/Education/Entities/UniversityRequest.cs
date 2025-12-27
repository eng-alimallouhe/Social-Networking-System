using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Enums;
using SNS.Domain.Common.Helpers;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Education.Entities;

public class UniversityRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) → Many(UniversityRequest)
    public Guid SubmitterProfileId { get; set; }

    // Properties
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public string ReviewComment { get; set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }

    // Navigation Properties
    public Profile SubmitterProfile { get; set; } = null!;
    public ICollection<ProfileUniversityRequest> Requests { get; set; } = new List<ProfileUniversityRequest>();

    public UniversityRequest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = RequestStatus.Pending;
    }
}