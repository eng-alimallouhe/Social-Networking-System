using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Educations.Entities;

public class AcademicRecord : Entity, IHardDeletable
{
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) to Many(Educations)
    public Guid ProfileId { get; private set; }

    // Foreign Key: One(University) to Many(Educations) - Optional 
    // Nullable, just in case you use the custom fallback pattern later
    public Guid UniversityId { get; private set; }

    // 3. The Details (Flexible Free Text)
    public string? Degree { get; private set; }        // e.g., "Bachelor's"
    public string FieldOfStudy { get; private set; } = string.Empty;  // e.g., "Pharmacy", "Software Engineering"
    public string? Grade { get; private set; }         // e.g., "3.8 GPA" or "Excellent"
    public string? Description { get; private set; }   // Let them talk about their graduation project!

    // 4. The Timeline
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsCurrent { get; private set; }

    //Navigation Property:
    public Profile Profile { get; set; } = null!;
    public University University { get; set; } = null!;

    private AcademicRecord()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static AcademicRecord Create(Guid profileId, Guid universityId, string? degree, string fieldOfStudy, string? grade, string? description, DateTime? startDate, DateTime? endDate, bool isCurrent)
    {
        var entity = new AcademicRecord();
        entity.ProfileId = profileId;
        entity.UniversityId = universityId;
        entity.Degree = degree;
        entity.FieldOfStudy = fieldOfStudy;
        entity.Grade = grade;
        entity.Description = description;
        entity.StartDate = startDate;
        entity.EndDate = endDate;
        entity.IsCurrent = isCurrent;
        return entity;
    }
}
