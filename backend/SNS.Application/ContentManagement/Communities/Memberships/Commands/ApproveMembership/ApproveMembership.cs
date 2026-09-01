using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.ContentManagement.Communities.Events;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Communities.Memberships.Commands.ApproveMembership;

/// <summary>
/// Represents a command to approve a pending community membership request.
/// </summary>
/// <param name="RequestId">The unique identifier of the join request.</param>
public sealed record ApproveMembershipCommand(
    Guid RequestId
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="ApproveMembershipCommand"/> to approve a join request and activate membership.
/// </summary>
internal sealed class ApproveMembershipCommandHandler : ICommandHandler<ApproveMembershipCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<CommunityMembership> _membershipRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public ApproveMembershipCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<CommunityMembership> membershipRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _dbContext = dbContext;
        _membershipRepo = membershipRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<Result> Handle(ApproveMembershipCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var joinRequest = await _dbContext.CommunityJoinRequests
            .Include(r => r.Community)
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (joinRequest == null || joinRequest.Status != JoinRequestStatus.Pending)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        var isOwner = joinRequest.Community.OwnerId == profileId.Value;
        var isModerator = !isOwner && await _dbContext.CommunityMemberships
            .AnyAsync(m => m.CommunityId == joinRequest.CommunityId &&
                           m.MemberId == profileId.Value &&
                           (m.Role == CommunityRole.Moderator || m.Role == CommunityRole.Owner) &&
                           m.Status == CommunityMembershipStatus.Active, cancellationToken);

        if (!isOwner && !isModerator)
        {
            return Result.Failure(SecurityStatusCodes.UnAuthorized);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            joinRequest.Approve();

            var existingMembership = await _dbContext.CommunityMemberships
                .FirstOrDefaultAsync(m => m.CommunityId == joinRequest.CommunityId && m.MemberId == joinRequest.SubmitterId, cancellationToken);

            if (existingMembership != null)
            {
                existingMembership.UpdateStatus(CommunityMembershipStatus.Active);
            }
            else
            {
                var membership = CommunityMembership.Create(joinRequest.SubmitterId, joinRequest.CommunityId, CommunityRole.Member, CommunityMembershipStatus.Active);
                _membershipRepo.Add(membership);
            }

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await _mediator.Publish(
                new DomainEventNotification<CommunityMemberJoinedIntegrationEvent>(
                    new CommunityMemberJoinedIntegrationEvent(joinRequest.CommunityId, joinRequest.SubmitterId, DateTime.UtcNow)),
                cancellationToken);

            return Result.Success(OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
