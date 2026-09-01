using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Memberships.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Communities.Memberships.Queries.GetMyMembership;

/// <summary>
/// Handles the execution of <see cref="GetMyMembershipQuery"/> to check membership and application state.
/// </summary>
internal sealed class GetMyMembershipQueryHandler : IQueryHandler<GetMyMembershipQuery, UserMembershipStatusDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetMyMembershipQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserMembershipStatusDto>> Handle(GetMyMembershipQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<UserMembershipStatusDto>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var communityExists = await _dbContext.Communities
            .AnyAsync(c => c.Id == request.CommunityId && c.IsActive, cancellationToken);

        if (!communityExists)
        {
            return Result<UserMembershipStatusDto>.Failure(ResourceStatusCode.NotFound);
        }

        var membership = await _dbContext.CommunityMemberships
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.CommunityId == request.CommunityId && m.MemberId == profileId.Value, cancellationToken);

        var hasPendingRequest = await _dbContext.CommunityJoinRequests
            .AnyAsync(r => r.CommunityId == request.CommunityId && r.SubmitterId == profileId.Value && r.Status == JoinRequestStatus.Pending, cancellationToken);

        var result = new UserMembershipStatusDto(
            IsMember: membership != null && membership.Status == CommunityMembershipStatus.Active,
            Role: membership?.Role,
            Status: membership?.Status,
            HasPendingRequest: hasPendingRequest
        );

        return Result<UserMembershipStatusDto>.Success(result, OperationStatusCode.Success);
    }
}
