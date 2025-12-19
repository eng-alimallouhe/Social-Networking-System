using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Enums;

namespace SNS.Domain.Content.Entities;

public class PostMedia : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key
    public Guid PostId { get; set; }

    // General
    public string Url { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
    public int Order { get; set; }
    public string? ThumbnailUrl { get; set; }
    public double? Duration { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }

    // Navigation
    public Post Post { get; set; } = null!;
}
