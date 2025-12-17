using SNS.Domain.Common.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Education.Entities;

public class FacultyRequest
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) → Many(FacultyRequest)
    public Guid SubmitterProfileId { get; set; }

    // Foreign Key: One(UniversityRequest) → Many(FacultyRequest) == Optional
    public Guid? UniversityRequestId { get; set; }

    // Foreign Key: One(University) → Many(FacultyRequest) == Optional
    public Guid? UniversityId { get; set; }

    // Properties
    public string FacultyName { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public string ReviewComment { get; set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }

    // Soft Delete
    public bool IsActive { get; set; }

    // Navigation Properties
    public Profile SubmitterProfile { get; set; } = null!;
    public University? University { get; set; }
    public UniversityRequest? UniversityRequest { get; set; }
    public ICollection<ProfileFacultyRequest> Requests { get; set; } = new List<ProfileFacultyRequest>();
}