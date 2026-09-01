using SNS.Application.ContentManagement.Comments.Comments.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.ContentManagement.Comments.Comments.Queries.GetUserComments;

public sealed record GetUserCommentsQuery(
    Guid? ProfileId = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<Paged<CommentSummaryDto>>;
