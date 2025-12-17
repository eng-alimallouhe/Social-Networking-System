using SNS.Domain.Communities.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Communities.Entities;

public class CommunityInvitation
    {
        // Primary Key
        public Guid Id { get; set; }

        // Foreign Key: One(Community) → Many(Invitations)
        public Guid CommunityId { get; set; }

        // Foreign Key: One(Profile) → Many(SentInvitations)
        public Guid InviterProfileId { get; set; }

        // Foreign Key: One(Profile) → Many(ReceivedInvitations)
        public Guid InviteeProfileId { get; set; }

        // Timestamp
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }

        public InvitationStatus InvitationStatus { get; set; }

        // Navigation Properties (Required)
        public Community Community { get; set; } = null!;
        public Profile InviterProfile { get; set; } = null!;
        public Profile InviteeProfile { get; set; } = null!;
    }
