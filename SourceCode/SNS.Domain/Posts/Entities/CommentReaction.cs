using SNS.Domain.Content.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Content.Entities;

public class CommentReaction
    {
        // Primary Key
        public Guid Id { get; set; }

        // Foreign Keys
        public Guid CommentId { get; set; }
        public Guid ReactingProfileId { get; set; }

        // General
        public ReactionType ReactionType { get; set; }

        // Timestamp
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Comment Comment { get; set; } = null!;
        public Profile Profile { get; set; } = null!;
    }

// Awaiting full transformation...
