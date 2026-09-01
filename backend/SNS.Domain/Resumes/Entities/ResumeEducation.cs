using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Resumes.Entities;

public class ResumeEducation : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Resume) To Many(Educations)
    public Guid ResumeId { get; private set; }

    public string UniversityName { get; private set; } = string.Empty;
    public string FacultyName { get; private set; } = string.Empty;
    public string Degree { get; private set; } = string.Empty;
    public string FieldOfStudy { get; private set; } = string.Empty;

    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public double? GPA { get; private set; }

    private ResumeEducation()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static ResumeEducation Create(Guid resumeId, string universityName, string facultyName, string degree, string fieldOfStudy, DateTime startDate, DateTime? endDate, double? gpa)
    {
        return new ResumeEducation
        {
            ResumeId = resumeId,
            UniversityName = universityName,
            FacultyName = facultyName,
            Degree = degree,
            FieldOfStudy = fieldOfStudy,
            GPA = gpa
        };
    }

    public void Update(string universityName, string facultyName, string degree, string fieldOfStudy, DateTime startDate, DateTime? endDate, double? gpa)
    {
        UniversityName = universityName;
        FacultyName = facultyName;
        Degree = degree;
        FieldOfStudy = fieldOfStudy;
        StartDate = startDate;
        EndDate = endDate;
        GPA = gpa;
    }
}
