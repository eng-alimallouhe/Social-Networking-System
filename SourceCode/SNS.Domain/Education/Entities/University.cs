using SNS.Domain.Abstractions.Common;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Education.Entities;

public class University : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Properties
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Soft Delete
    public bool IsActive { get; set; }

    // Navigation Properties
    public ICollection<Faculty> Faculties { get; set; } = new List<Faculty>();
    public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
    public ICollection<FacultyRequest> FacultyRequests { get; set; } = new List<FacultyRequest>();
}