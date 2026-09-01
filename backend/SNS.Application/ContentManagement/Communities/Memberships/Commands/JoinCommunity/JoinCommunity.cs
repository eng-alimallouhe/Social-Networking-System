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

namespace SNS.Application.ContentManagement.Communities.Memberships.Commands.JoinCommunity;

/// <summary>
/// Represents a command to join a public community or submit a join request for a private community.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
/// <param name="Notes">Optional application notes when requesting to join a private community.</param>
public sealed record JoinCommunityCommand(
    Guid CommunityId,
    string? Notes = null
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="JoinCommunityCommand"/> to join or request membership in a community.
/// </summary>
internal sealed class JoinCommunityCommandHandler : ICommandHandler<JoinCommunityCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<CommunityMembership> _membershipRepo;
    private readonly IRepository<CommunityJoinRequest> _joinRequestRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public JoinCommunityCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<CommunityMembership> membershipRepo,
        IRepository<CommunityJoinRequest> joinRequestRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _dbContext = dbContext;
        _membershipRepo = membershipRepo;
        _joinRequestRepo = joinRequestRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<Result> Handle(JoinCommunityCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var community = await _dbContext.Communities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CommunityId && c.IsActive, cancellationToken);

        if (community == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        var existingMembership = await _dbContext.CommunityMemberships
            .FirstOrDefaultAsync(m => m.CommunityId == request.CommunityId && m.MemberId == profileId.Value, cancellationToken);

        if (existingMembership != null && existingMembership.Status == CommunityMembershipStatus.Active)
        {
            return Result.Failure(OperationStatusCode.Conflict);
        }

        if (community.Type == CommunityType.Public)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                if (existingMembership != null)
                {
                    existingMembership.UpdateStatus(CommunityMembershipStatus.Active);
                }
                else
                {
                    var membership = CommunityMembership.Create(profileId.Value, request.CommunityId, CommunityRole.Member, CommunityMembershipStatus.Active);
                    _membershipRepo.Add(membership);
                }

                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                await _mediator.Publish(
                    new DomainEventNotification<CommunityMemberJoinedIntegrationEvent>(
                        new CommunityMemberJoinedIntegrationEvent(request.CommunityId, profileId.Value, DateTime.UtcNow)),
                    cancellationToken);

                return Result.Success(OperationStatusCode.Success);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
        else
        {
            var pendingRequest = await _dbContext.CommunityJoinRequests
                .FirstOrDefaultAsync(r => r.CommunityId == request.CommunityId && r.SubmitterId == profileId.Value && r.Status == JoinRequestStatus.Pending, cancellationToken);

            if (pendingRequest != null)
            {
                return Result.Failure(OperationStatusCode.Conflict);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var joinRequest = CommunityJoinRequest.Create(request.CommunityId, profileId.Value, request.Notes ?? string.Empty);
                _joinRequestRepo.Add(joinRequest);

                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                await _mediator.Publish(
                    new DomainEventNotification<CommunityMembershipRequestedIntegrationEvent>(
                        new CommunityMembershipRequestedIntegrationEvent(request.CommunityId, profileId.Value, DateTime.UtcNow)),
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
}
