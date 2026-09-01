using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Comments.Entities;
using SNS.Domain.ContentManagement.Comments.Events;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Comments.Comments.Commands.DeleteComment;

public sealed record DeleteCommentCommand(
    Guid CommentId
) : ICommand;

internal sealed class DeleteCommentCommandHandler : ICommandHandler<DeleteCommentCommand>
{
    private readonly ISoftDeletableRepository<Comment> _commentRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public DeleteCommentCommandHandler(
        ISoftDeletableRepository<Comment> commentRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _commentRepo = commentRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var comment = await _commentRepo.GetByIdAsync(request.CommentId, cancellationToken);
        if (comment == null || !comment.IsActive)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        if (comment.AuthorId != profileId.Value)
        {
            return Result.Failure(SecurityStatusCodes.UnAuthorized);
        }

        comment.SoftDelete();
        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(
            new DomainEventNotification<CommentDeletedIntegrationEvent>(
                new CommentDeletedIntegrationEvent(comment.AuthorId, comment.Id, comment.PostId, DateTime.UtcNow)),
            cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}
