using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.ContentManagement.Posts.Posts.Queries.GetUserReactedPosts;

public sealed record GetUserReactedPostsQuery(
    int Page = 1,
    int PageSize = 10
) : IQuery<Paged<PostOverviewDto>>;
