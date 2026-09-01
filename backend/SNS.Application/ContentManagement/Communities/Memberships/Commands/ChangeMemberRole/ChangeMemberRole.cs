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

namespace SNS.Application.ContentManagement.Communities.Memberships.Commands.ChangeMemberRole;

/// <summary>
/// Represents a command to change the role of a community member.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
/// <param name="MemberProfileId">The profile identifier of the member.</param>
/// <param name="NewRole">The new role to assign to the member.</param>
public sealed record ChangeMemberRoleCommand(
    Guid CommunityId,
    Guid MemberProfileId,
    CommunityRole NewRole
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="ChangeMemberRoleCommand"/> to update a member's role in the community.
/// </summary>
internal sealed class ChangeMemberRoleCommandHandler : ICommandHandler<ChangeMemberRoleCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ChangeMemberRoleCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ChangeMemberRoleCommand request, CancellationToken cancellationToken)
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
            targetMembership.ChangeRole(request.NewRole);
        }
        else if (callerMembership != null && callerMembership.Role == CommunityRole.Moderator)
        {
            if (request.NewRole == CommunityRole.Owner || targetMembership.Role == CommunityRole.Owner || targetMembership.Role == CommunityRole.Moderator)
            {
                return Result.Failure(SecurityStatusCodes.UnAuthorized);
            }
            targetMembership.ChangeRole(request.NewRole);
        }
        else
        {
            return Result.Failure(SecurityStatusCodes.UnAuthorized);
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}
