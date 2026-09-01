using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Memberships.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Communities.Memberships.Queries.GetMembershipRequests;

/// <summary>
/// Handles the execution of <see cref="GetMembershipRequestsQuery"/> to retrieve pending join requests for community owners/moderators.
/// </summary>
internal sealed class GetMembershipRequestsQueryHandler : IQueryHandler<GetMembershipRequestsQuery, Paged<MembershipRequestDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetMembershipRequestsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<MembershipRequestDto>>> Handle(GetMembershipRequestsQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<Paged<MembershipRequestDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var community = await _dbContext.Communities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CommunityId && c.IsActive, cancellationToken);

        if (community == null)
        {
            return Result<Paged<MembershipRequestDto>>.Failure(ResourceStatusCode.NotFound);
        }

        var isOwner = community.OwnerId == profileId.Value;
        var isModerator = !isOwner && await _dbContext.CommunityMemberships
            .AnyAsync(m => m.CommunityId == request.CommunityId &&
                           m.MemberId == profileId.Value &&
                           (m.Role == CommunityRole.Moderator || m.Role == CommunityRole.Owner) &&
                           m.Status == CommunityMembershipStatus.Active, cancellationToken);

        if (!isOwner && !isModerator)
        {
            return Result<Paged<MembershipRequestDto>>.Failure(SecurityStatusCodes.UnAuthorized);
        }

        var page = request.Page > 0 ? request.Page : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 20;
        if (pageSize > 100) pageSize = 100;

        var baseQuery = _dbContext.CommunityJoinRequests
            .AsNoTracking()
            .Where(r => r.CommunityId == request.CommunityId && r.Status == JoinRequestStatus.Pending);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var rawRequests = await baseQuery
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new
            {
                r.Id,
                r.CommunityId,
                r.SubmitterId,
                r.Status,
                r.Notes,
                r.CreatedAt,
                SubmitterFullName = r.Submitter.FullName,
                SubmitterSpecialization = r.Submitter.Specialization,
                SubmitterAvatarKey = r.Submitter.ProfilePictureObjectKey
            })
            .ToListAsync(cancellationToken);

        var distinctKeys = rawRequests
            .Select(r => r.SubmitterAvatarKey)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct()
            .ToList();

        var urlTasks = distinctKeys.Select(async k => new
        {
            Key = k!,
            Url = await _fileStorageService.GetTemporaryUrlAsync(k!, TimeSpan.FromHours(1))
        });
        var resolvedUrls = await Task.WhenAll(urlTasks);
        var urlMap = resolvedUrls.ToDictionary(r => r.Key, r => r.Url);

        var items = rawRequests.Select(r => new MembershipRequestDto(
            RequestId: r.Id,
            CommunityId: r.CommunityId,
            Submitter: new ProfileSnapshotDto(
                r.SubmitterId,
                r.SubmitterFullName,
                r.SubmitterSpecialization,
                !string.IsNullOrWhiteSpace(r.SubmitterAvatarKey) && urlMap.TryGetValue(r.SubmitterAvatarKey, out var avatarUrl) ? avatarUrl : null
            ),
            Status: r.Status,
            Notes: r.Notes,
            CreatedAt: r.CreatedAt
        )).ToList();

        var pagedResult = new Paged<MembershipRequestDto>(
            items,
            totalCount,
            pageSize,
            page
        );

        return Result<Paged<MembershipRequestDto>>.Success(pagedResult, OperationStatusCode.Success);
    }
}
