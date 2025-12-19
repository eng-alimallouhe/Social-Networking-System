using SNS.Domain.Abstractions.Common;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Education.Entities;

public class Faculty : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(University) → Many(Faculty)
    public Guid UniversityId { get; set; }

    // Properties
    public string Name { get; set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Soft Delete
    public bool IsActive { get; set; }

    // Navigation Properties
    public University University { get; set; } = null!;
    public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}