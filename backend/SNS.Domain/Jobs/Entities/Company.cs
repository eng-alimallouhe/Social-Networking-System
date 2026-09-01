using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Jobs.Entities;

public class Company : Entity, ISoftDeletable
{
    // Primary Key: 
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string Industry { get; private set; } = string.Empty;
    public string? WebsiteUrl { get; private set; }
    public string? LogoObjectKey { get; private set; }

    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public ICollection<Job> PostedJobs { get; set; } = new List<Job>();
    public ICollection<CompanyAdministrator> Administrators { get; set; } = new List<CompanyAdministrator>();

    private Company()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static Company Create(string name, string industry, string? websiteUrl = null, string? logoObjectKey = null)
    {
        var entity = new Company
        {
            Name = name,
            Industry = industry,
            WebsiteUrl = websiteUrl,
            LogoObjectKey = logoObjectKey,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        return entity;
    }

    public void Update(string name, string industry, string? websiteUrl, string? logoObjectKey)
    {
        Name = name;
        Industry = industry;
        WebsiteUrl = websiteUrl;
        LogoObjectKey = logoObjectKey;
    }

    public void SoftDelete()
    {
        IsActive = false;
    }
}
