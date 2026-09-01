using SNS.Application.ContentManagement.Posts.PostReactions.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.ContentManagement.Posts.PostReactions.Queries.GetPostReactions;

public sealed record GetPostReactionsQuery(
    Guid PostId,
    int Page = 1,
    int PageSize = 20
) : IQuery<Paged<PostReactionSummaryDto>>;
