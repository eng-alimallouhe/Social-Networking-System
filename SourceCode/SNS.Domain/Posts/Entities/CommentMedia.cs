using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Enums;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Content.Entities;

namespace SNS.Domain.Posts.Entities;

public class CommentMedia : IHardDeletable  
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key
    public Guid CommentId { get; set; }

    // General
    public string Url { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public MediaType Type { get; set; }
    public int Order { get; set; }
    public string? ThumbnailUrl { get; set; }
    public double? Duration { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }

    // Navigation
    public Comment Comment { get; set; } = null!;

    public CommentMedia()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}
