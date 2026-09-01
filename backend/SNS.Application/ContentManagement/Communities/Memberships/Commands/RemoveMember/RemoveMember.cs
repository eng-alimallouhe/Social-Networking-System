using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Communities.Memberships.Commands.RemoveMember;

/// <summary>
/// Represents a command to remove a member from a community by owner or moderator.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
/// <param name="MemberProfileId">The profile identifier of the member to remove.</param>
public sealed record RemoveMemberCommand(
    Guid CommunityId,
    Guid MemberProfileId
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="RemoveMemberCommand"/> to remove a membership record.
/// </summary>
internal sealed class RemoveMemberCommandHandler : ICommandHandler<RemoveMemberCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<CommunityMembership> _membershipRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RemoveMemberCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<CommunityMembership> membershipRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _membershipRepo = membershipRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
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

        if (request.MemberProfileId == community.OwnerId)
        {
            return Result.Failure(OperationStatusCode.Failure);
        }

        var isOwner = community.OwnerId == profileId.Value;
        var callerMembership = await _dbContext.CommunityMemberships
            .FirstOrDefaultAsync(m => m.CommunityId == request.CommunityId && m.MemberId == profileId.Value && m.Status == CommunityMembershipStatus.Active, cancellationToken);

        var targetMembership = await _dbContext.CommunityMemberships
            .FirstOrDefaultAsync(m => m.CommunityId == request.CommunityId && m.MemberId == request.MemberProfileId && m.Status == CommunityMembershipStatus.Active, cancellationToken);

        if (targetMembership == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        if (isOwner)
        {
            // Owner can remove any member
        }
        else if (callerMembership != null && callerMembership.Role == CommunityRole.Moderator)
        {
            if (targetMembership.Role == CommunityRole.Moderator || targetMembership.Role == CommunityRole.Owner)
            {
                return Result.Failure(SecurityStatusCodes.UnAuthorized);
            }
        }
        else
        {
            return Result.Failure(SecurityStatusCodes.UnAuthorized);
        }

        _membershipRepo.Delete(targetMembership);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}
