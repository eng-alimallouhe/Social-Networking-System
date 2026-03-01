using SNS.Domain.Common.Helpers;
using SNS.Domain.Abstractions.Common;
using SNS.Domain.QA.Enums;

namespace SNS.Domain.Jobs.Entities;


public class Job : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) ? Many(Jobs)
    public Guid OwnerId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public JobType Type { get; set; }
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public SalaryTyp SalaryType { get; set; }
    public string KeyResponsibilitiesText { get; set; } = string.Empty;

    //Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    //Soft Delete
    public bool IsActive { get; set; }

    //Navigation Properties
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();

    public Job()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }
}

