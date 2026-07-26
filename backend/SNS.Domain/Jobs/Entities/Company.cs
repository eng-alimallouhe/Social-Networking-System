using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Jobs.Entities;

public class Company : Entity, ISoftDeletable
{
    //Primary Key: 
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
    }

    public static Company Create(string name, string industry, string? websiteUrl, string? logoObjectKey)
    {
        var entity = new Company()
        {
            Name = name,
            Industry = industry
        };
        entity.WebsiteUrl = websiteUrl;
        entity.LogoObjectKey = logoObjectKey;
        return entity;
    }

    public void SoftDelete()
    {
        this.IsActive = false;
    }
}
