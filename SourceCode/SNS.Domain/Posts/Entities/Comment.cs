using SNS.Domain.SocialGraph;

namespace SNS.Domain.Content.Entities;

public class Comment
    {
        // Primary Key
        public Guid Id { get; set; }

        // Foreign Keys
        public Guid AuthorProfileId { get; set; }
        public Guid PostId { get; set; }
        public Guid? ParentCommentId { get; set; }

        // General
        public string Content { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        // Timestamp
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public Post Post { get; set; } = null!;
        public Profile AuthorProfile { get; set; } = null!;
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
        public ICollection<CommentReaction> Reactions { get; set; } = new List<CommentReaction>();
    }

// Awaiting full transformation...
