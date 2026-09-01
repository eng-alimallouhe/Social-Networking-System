using SNS.Application.ContentManagement.Comments.Comments.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.ContentManagement.Comments.Comments.Queries.GetCommentReplies;

public sealed record GetCommentRepliesQuery(
    Guid CommentId,
    int Page = 1,
    int PageSize = 10
) : IQuery<Paged<CommentSummaryDto>>;
