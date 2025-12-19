using SNS.Domain.Abstractions.Common;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Education.Entities;

public class ProfileFacultyRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) → Many(ProfileFacultyRequest)
    public Guid ProfileId { get; set; }

    // Foreign Key: One(FacultyRequest) → Many(ProfileFacultyRequest)
    public Guid FacultyRequestId { get; set; }

    // Properties

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    public Profile Profile { get; set; } = null!;
    public FacultyRequest FacultyRequest { get; set; } = null!;
}