using SNS.Domain.Communities.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Communities.Entities;

public class CommunityJoinRequest
    {
        // Primary Key
        public Guid Id { get; set; }

        // Foreign Key: One(Community) → Many(JoinRequests)
        public Guid CommunityId { get; set; }

        // Foreign Key: One(Profile) → Many(JoinRequests)
        public Guid ProfileId { get; set; }

        public JoinRequestStatus JoinRequestStatus { get; set; }
        public string Notes { get; set; } = string.Empty;

        // Timestamp
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }

        // Navigation Properties (Required)
        public Community Community { get; set; } = null!;
        public Profile Profile { get; set; } = null!;
    }
