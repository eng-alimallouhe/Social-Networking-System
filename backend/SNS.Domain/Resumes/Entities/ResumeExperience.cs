using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Resumes.Entities;

public class ResumeExperience : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key
    public Guid ResumeId { get; private set; }

    public string CompanyName { get; private set; } = string.Empty;
    public string Position { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    private ResumeExperience()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static ResumeExperience Create(Guid resumeId, string companyName, string position, string description, DateTime startDate, DateTime? endDate)
    {
        return new ResumeExperience
        {
            ResumeId = resumeId,
            CompanyName = companyName,
            Position = position,
            Description = description,
            StartDate = startDate,
            EndDate = endDate
        };
    }
}
