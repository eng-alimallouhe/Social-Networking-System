using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Communities.Entities;

public class Community : Entity, ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) ? Many(Communities)
    public Guid OwnerId { get; private set; }

    //Unique Name
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string RulesText { get; private set; } = string.Empty;
    public ModerationPolicy Policy { get; private set; }
    public CommunityType Type { get; private set; }
    public CommunityStatus Status { get; private set; }
    public string LogoUrl { get; private set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; private set; } = DateTime.UtcNow;

    // Soft Delete
    public bool IsActive { get; private set; } = true;

    // Navigation Properties (Required)
    public CommunitySettings Settings { get; private set; } = null!;

    // Navigation Properties
    public ICollection<CommunityMembership> Memberships { get; private set; } = new List<CommunityMembership>();
    public ICollection<CommunityRule> Rules { get; private set; } = new List<CommunityRule>();
    public ICollection<CommunityAuditLog> AuditLogs { get; private set; } = new List<CommunityAuditLog>();


    private Community()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        UpdateAt = DateTime.UtcNow;
    }

    public static Community Create(Guid ownerId, string name, string description, string rulesText, ModerationPolicy policy, CommunityType type, CommunityStatus status, string logoUrl)
    {
        var entity = new Community();
        entity.OwnerId = ownerId;
        entity.Name = name;
        entity.Description = description;
        entity.RulesText = rulesText;
        entity.Policy = policy;
        entity.Type = type;
        entity.Status = status;
        entity.LogoUrl = logoUrl;
        return entity;
    }

    public void SoftDelete()
    {
        this.IsActive = false;
    }
}


