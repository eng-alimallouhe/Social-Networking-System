using SNS.Domain.Shared.Enums;

namespace SNS.Application.ContentManagement.Posts.Posts.Contracts;

/// <summary>
/// Represents a media item attachment with resolved temporary URL.
/// </summary>
public sealed record PostMediaDto(
    string Url,
    int Order,
    MediaType Type
);
