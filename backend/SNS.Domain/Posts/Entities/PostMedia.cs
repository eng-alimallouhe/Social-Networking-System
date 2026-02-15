using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Enums;
using SNS.Domain.Common.Helpers;

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
    public MediaType Type { get; set; }
    public int Order { get; set; }
    public string? ThumbnailUrl { get; set; }
    public double? Duration { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }

    // Navigation
    public Post Post { get; set; } = null!;

    public PostMedia()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}