using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.ContentManagement.Communities.Events;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Communities.Communities.Commands.DeleteCommunity;

/// <summary>
/// Represents a command to soft-delete a community by its owner.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community to delete.</param>
public sealed record DeleteCommunityCommand(
    Guid CommunityId
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="DeleteCommunityCommand"/> to soft-delete a community.
/// </summary>
internal sealed class DeleteCommunityCommandHandler : ICommandHandler<DeleteCommunityCommand>
{
    private readonly ISoftDeletableRepository<Community> _communityRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public DeleteCommunityCommandHandler(
        ISoftDeletableRepository<Community> communityRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _communityRepo = communityRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteCommunityCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var community = await _communityRepo.GetByIdAsync(request.CommunityId, cancellationToken);
        if (community == null || !community.IsActive)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        if (community.OwnerId != profileId.Value)
        {
            return Result.Failure(SecurityStatusCodes.UnAuthorized);
        }

        community.SoftDelete();
        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(
            new DomainEventNotification<CommunityDeletedIntegrationEvent>(
                new CommunityDeletedIntegrationEvent(community.Id, DateTime.UtcNow)),
            cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}
