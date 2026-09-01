using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Communities.Memberships.Commands.RejectMembership;

/// <summary>
/// Represents a command to reject a pending community membership request.
/// </summary>
/// <param name="RequestId">The unique identifier of the join request.</param>
public sealed record RejectMembershipCommand(
    Guid RequestId
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="RejectMembershipCommand"/> to reject a membership application.
/// </summary>
internal sealed class RejectMembershipCommandHandler : ICommandHandler<RejectMembershipCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RejectMembershipCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(RejectMembershipCommand request, CancellationToken cancellationToken)
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

        joinRequest.Reject();
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}
