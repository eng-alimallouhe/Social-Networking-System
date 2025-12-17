using SNS.Domain.SocialGraph;

namespace SNS.Domain.Education.Entities;

public class ProfileUniversityRequest
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(UniversityRequest) → Many(ProfileUniversityRequest)
    public Guid UniversityRequestId { get; set; }

    // Foreign Key: One(Profile) → Many(ProfileUniversityRequest)
    public Guid ProfileId { get; set; }

    // Properties

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Soft Delete
    public bool IsActive { get; set; }

    // Navigation Properties
    public Profile Profile { get; set; } = null!;
    public UniversityRequest UniversityRequest { get; set; } = null!;
}