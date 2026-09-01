using SNS.Application.ContentManagement.Comments.Comments.Contracts;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.ContentManagement.Posts.PostMentions.Contracts;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.DTOs;
using SNS.Domain.ContentManagement.Posts.Enums;

namespace SNS.Application.ContentManagement.Posts.Posts.Contracts;

public sealed record PostDetailsDto(
    Guid Id,
    string Title,
    string Content,
    bool IsPinned,
    PostType Type,
    PostStatus? Status,
    int EngagementScore,
    int SaveCount,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    ProfileSnapshotDto Author,
    CommunitySnapshotDto? Community,
    List<PostMediaDto> Media,
    Paged<CommentSummaryDto> Comments,
    List<string> Tags,
    int ReactionCount,
    List<PostMentionDto> Mentions
);
