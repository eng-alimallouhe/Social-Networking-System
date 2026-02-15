using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Communities.Enums;
using SNS.Domain.Content.Entities;
using SNS.Domain.QA.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Communities.Entities;

public class Community : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) → Many(Communities)
    public Guid OwnerId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RulesText { get; set; } = string.Empty;
    public ModerationPolicy Policy { get; set; }
    public CommunityType Type { get; set; }
    public CommunityStatus Status { get; set; }
    public string LogoUrl { get; set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

    // Soft Delete
    public bool IsActive { get; set; } = true;

    // Navigation Properties (Required)
    public Profile Owner { get; set; } = default!;
    public CommunitySettings Settings { get; set; } = null!;

    // Navigation Properties
    public ICollection<CommunityMembership> Memberships { get; set; } = new List<CommunityMembership>();
    public ICollection<CommunityRule> Rules { get; set; } = new List<CommunityRule>();
    public ICollection<CommunityAuditLog> AuditLogs { get; set; } = new List<CommunityAuditLog>();
    public ICollection<CommunityCreationRequest> CreationRequests { get; set; } = new List<CommunityCreationRequest>();
    
    public ICollection<Problem> Problems { get; set; } = new List<Problem>();
    
    public ICollection<Post> Posts { get; set; } 
        = new List<Post>();


    public Community()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        UpdateAt = DateTime.UtcNow;
    }
}
