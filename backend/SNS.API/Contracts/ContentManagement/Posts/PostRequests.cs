using Microsoft.AspNetCore.Http;

namespace SNS.API.Contracts.ContentManagement.Posts;

/// <summary>
/// Represents a multipart/form-data request for creating a new post.
/// </summary>
public sealed record CreatePostRequest(
    Guid? CommunityId,
    string Title,
    string Content,
    bool IsPenned,
    List<IFormFile>? Files,
    List<Guid>? MentionedProfileIds
);
