using SNS.Domain.Abstractions.Common;

namespace SNS.Domain.Resumes.Entities;

public class ResumeEducation : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Resume) To Many(Educations)
    public Guid ResumeId { get; set; }

    public string UniversityName { get; set; } = string.Empty;
    public string FacultyName { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public double? GPA { get; set; }

    // Navigation
    public Resume Resume { get; set; } = null!;
}
