using SNS.Domain.Communities.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Communities.Entities;


    public class CommunityCreationRequest
    {
        // Primary Key
        public Guid Id { get; set; }

        // Foreign Key: One(Profile) → Many(CreationRequests)
        public Guid SubmitterProfileId { get; set; }

        public string CommunityName { get; set; } = string.Empty;
        public string CommunityDescription { get; set; } = string.Empty;
        public string RulesText { get; set; } = string.Empty;
        public ModerationPolicy Policy { get; set; }
        public CommunityType CommunityType { get; set; }
        public CommunityStatus Status { get; set; }

        // Timestamp
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }

        public string ReviewNotes { get; set; } = string.Empty;

        // Navigation Properties (Required)
        public Profile SubmitterProfile { get; set; } = null!;
    }
