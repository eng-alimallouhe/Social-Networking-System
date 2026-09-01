using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.ContentManagement.Posts.PostSaves.Queries.GetSavedPosts;

public sealed record GetSavedPostsQuery(
    int Page = 1,
    int PageSize = 10
) : IQuery<Paged<PostOverviewDto>>;
