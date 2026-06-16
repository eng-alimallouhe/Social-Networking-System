namespace SNS.Domain.Search.Documents;

public class ProfileDocument
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public List<string> Universities { get; set; } = new List<string>();
    public AcademicRecordDocument AcademicRecordDocument { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingsCount { get; set; }
    public int Reputation { get; set; }
    public List<Guid> BlackList { get; set; } = new List<Guid>();
    public List<string> Skills { get; set; } = new List<string>();
}


public class AcademicRecordDocument
{
    public string UniversityName { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
}
