using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.ContentManagement.Posts.Events;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Posts.Posts.Commands.DeletePost;

public sealed record DeletePostCommand(Guid PostId) : ICommand;

internal sealed class DeletePostCommandHandler : ICommandHandler<DeletePostCommand>
{
    private readonly ISoftDeletableRepository<Post> _postRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public DeletePostCommandHandler(
        ISoftDeletableRepository<Post> postRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _postRepo = postRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var post = await _postRepo.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        if (post.AuthorId != profileId.Value)
        {
            return Result.Failure(SecurityStatusCodes.UnAuthorized);
        }

        _postRepo.SoftDelete(post);
        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(
            new DomainEventNotification<PostDeletedIntegrationEvent>(
                new PostDeletedIntegrationEvent(post.AuthorId, post.Id, DateTime.UtcNow)),
            cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}
