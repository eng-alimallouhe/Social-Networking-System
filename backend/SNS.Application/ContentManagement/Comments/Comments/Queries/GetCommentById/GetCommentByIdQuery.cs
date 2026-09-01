using SNS.Application.ContentManagement.Comments.Comments.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.ContentManagement.Comments.Comments.Queries.GetCommentById;

public sealed record GetCommentByIdQuery(Guid CommentId) : IQuery<CommentDetailsDto>;
