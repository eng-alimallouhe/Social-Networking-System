using SNS.Application.ContentManagement.Comments.CommentReactions.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.ContentManagement.Comments.CommentReactions.Queries.GetCommentReactions;

public sealed record GetCommentReactionsQuery(
    Guid CommentId,
    int Page = 1,
    int PageSize = 20
) : IQuery<Paged<CommentReactionSummaryDto>>;
