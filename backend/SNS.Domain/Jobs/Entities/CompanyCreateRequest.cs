using SNS.Domain.Jobs.Enums;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Jobs.Entities;

public class CompanyCreateRequest : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) → Many(CompanyCreateRequests)
    public Guid ProfileId { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string Industry { get; private set; } = string.Empty;
    public string? WebsiteUrl { get; private set; }
    public string? LogoObjectKey { get; private set; }

    public CompanyCreateRequestStatus Status { get; private set; }

    public Guid? CreatedCompanyId { get; private set; }
    public Guid? ReviewedByProfileId { get; private set; }
    public string? ReviewNote { get; private set; }

    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }

    // Navigation Properties
    public Profile Profile { get; private set; } = null!;

    private CompanyCreateRequest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = CompanyCreateRequestStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public static CompanyCreateRequest Create(
        Guid profileId,
        string name,
        string industry,
        string? websiteUrl = null,
        string? logoObjectKey = null)
    {
        return new CompanyCreateRequest
        {
            ProfileId = profileId,
            Name = name,
            Industry = industry,
            WebsiteUrl = websiteUrl,
            LogoObjectKey = logoObjectKey
        };
    }

    public void Approve(Guid createdCompanyId, Guid reviewedByProfileId, string? reviewNote = null)
    {
        Status = CompanyCreateRequestStatus.Approved;
        CreatedCompanyId = createdCompanyId;
        ReviewedByProfileId = reviewedByProfileId;
        ReviewNote = reviewNote;
        ReviewedAt = DateTime.UtcNow;
    }

    public void Reject(Guid reviewedByProfileId, string? reviewNote = null)
    {
        Status = CompanyCreateRequestStatus.Rejected;
        ReviewedByProfileId = reviewedByProfileId;
        ReviewNote = reviewNote;
        ReviewedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = CompanyCreateRequestStatus.Cancelled;
        ReviewedAt = DateTime.UtcNow;
    }
}
