using SNS.Domain.QA.Enums;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Jobs.Entities;


public class Job : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) ? Many(Jobs)
    public Guid CompanyId { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public JobType Type { get; private set; }
    public decimal MinSalary { get; private set; }
    public decimal MaxSalary { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public SalaryTyp SalaryType { get; private set; }
    public string KeyResponsibilitiesText { get; private set; } = string.Empty;

    //Timestamp
    public DateTime CreatedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }

    //Soft Delete
    public bool IsActive { get; private set; }

    //Navigation Properties
    public Company Company { get; private set; } = null!;
    public ICollection<JobApplication> Applications { get; private set; } = new List<JobApplication>();
    public ICollection<JobSkill> JobSkills { get; private set; } = new List<JobSkill>();

    private Job()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static Job Create(string title, string description, Guid companyId, string location, JobType type,
     decimal minSalary, decimal maxSalary, string currencyCode, SalaryTyp salaryType, string keyResponsibilitiesText)
    {
        return new Job
        {
            Title = title,
            Description = description,
            Location = location,
            Type = type,
            CompanyId = companyId,
            MinSalary = minSalary,
            MaxSalary = maxSalary,
            CurrencyCode = currencyCode,
            SalaryType = salaryType,
            KeyResponsibilitiesText = keyResponsibilitiesText
        };
    }

    public void SoftDelete()
    {
        if (IsActive)
        {
            IsActive = false;
            ClosedAt = DateTime.UtcNow;
        }
    }
}

