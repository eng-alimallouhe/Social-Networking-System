using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Comments.Entities;

public class CommentMedia : Entity, IHardDeletable  
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Comment) to Many(CommentMedia)
    public Guid CommentId { get; private set; }

    // General
    public string Url { get; private set; } = string.Empty;
    public string MimeType { get; private set; } = string.Empty;
    public MediaType Type { get; private set; }
    public int Order { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public double? Duration { get; private set; }
    public int? Width { get; private set; }
    public int? Height { get; private set; }

    private CommentMedia()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static CommentMedia Create(Guid commentId, string url, string mimeType, MediaType type, int order, string? thumbnailUrl, double? duration, int? width, int? height)
    {
        var entity = new CommentMedia();
        entity.CommentId = commentId;
        entity.Url = url;
        entity.MimeType = mimeType;
        entity.Type = type;
        entity.Order = order;
        entity.ThumbnailUrl = thumbnailUrl;
        entity.Duration = duration;
        entity.Width = width;
        entity.Height = height;
        return entity;
    }
}

