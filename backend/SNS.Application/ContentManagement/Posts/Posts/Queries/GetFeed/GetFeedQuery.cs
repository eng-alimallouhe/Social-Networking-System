using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.ContentManagement.Posts.Posts.Queries.GetFeed;

public sealed record GetFeedQuery(
    int CurrentPage = 1,
    int PageSize = 10
) : IQuery<List<PostOverviewDto>>;
