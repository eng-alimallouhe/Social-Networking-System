namespace SNS.Domain.Communities.Entities;

public class CommunitySettings
    {
        // Primary Key
        public Guid Id { get; set; }

        // Foreign Key: One(Community) → One(Settings)
        public Guid CommunityId { get; set; }

        public bool AllowPostWithoutApproval { get; set; }
        public bool AllowInvitationsByMembers { get; set; }
        public bool AllowComments { get; set; }
        public bool AllowMediaUpload { get; set; }

        // Navigation Properties (Required)
        public Community Community { get; set; } = null!;
    }
