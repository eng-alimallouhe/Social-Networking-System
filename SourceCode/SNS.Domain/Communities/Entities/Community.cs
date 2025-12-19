using SNS.Domain.Abstractions.Common;
using SNS.Domain.Communities.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Communities.Entities;

public class Community : ISoftDeletable
    {
        // Primary Key
        public Guid Id { get; set; }

        // Foreign Key: One(Profile) → Many(Communities)
        public Guid OwnerId { get; set; }

        public string CommunityName { get; set; } = string.Empty;
        public string CommunityDescription { get; set; } = string.Empty;
        public string RulesText { get; set; } = string.Empty;
        public ModerationPolicy Policy { get; set; }
        public CommunityType CommunityType { get; set; }
        public CommunityStatus CommunityStatus { get; set; }
        public string CommunityLogoUrl { get; set; } = string.Empty;

        // Timestamp
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Soft Delete
        public bool IsActive { get; set; } = true;

        // Navigation Properties (Required)
        public Profile Owner { get; set; } = null!;
        public CommunitySettings Settings { get; set; } = null!;

        // Navigation Properties
        public ICollection<CommunityMembership> Memberships { get; set; } = new List<CommunityMembership>();
        public ICollection<CommunityRule> Rules { get; set; } = new List<CommunityRule>();
        public ICollection<CommunityAuditLog> AuditLogs { get; set; } = new List<CommunityAuditLog>();
        public ICollection<CommunityCreationRequest> CreationRequests { get; set; } = new List<CommunityCreationRequest>();
    }
