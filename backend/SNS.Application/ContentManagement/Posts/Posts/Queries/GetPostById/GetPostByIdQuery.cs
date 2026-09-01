using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.ContentManagement.Posts.Posts.Queries.GetPostById;

public sealed record GetPostByIdQuery(Guid PostId) : IQuery<PostDetailsDto>;
