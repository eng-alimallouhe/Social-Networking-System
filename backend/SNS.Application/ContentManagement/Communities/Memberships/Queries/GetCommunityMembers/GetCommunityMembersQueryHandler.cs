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

namespace SNS.Application.ContentManagement.Communities.Memberships.Queries.GetCommunityMembers;

/// <summary>
/// Handles the execution of <see cref="GetCommunityMembersQuery"/> to retrieve paginated community members with profile snapshots.
/// </summary>
internal sealed class GetCommunityMembersQueryHandler : IQueryHandler<GetCommunityMembersQuery, Paged<CommunityMemberDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;

    public GetCommunityMembersQueryHandler(
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Paged<CommunityMemberDto>>> Handle(GetCommunityMembersQuery request, CancellationToken cancellationToken)
    {
        var community = await _dbContext.Communities
            .AsNoTracking()
            .Where(c => c.Id == request.CommunityId && c.IsActive)
            .Select(c => new { c.Id, c.Type, c.OwnerId })
            .FirstOrDefaultAsync(cancellationToken);

        if (community == null)
        {
            return Result<Paged<CommunityMemberDto>>.Failure(ResourceStatusCode.NotFound);
        }

        var profileId = _currentUserService.ProfileId;
        if (community.Type == CommunityType.Private)
        {
            var isMember = profileId.HasValue && (community.OwnerId == profileId.Value || await _dbContext.CommunityMemberships.AnyAsync(m => m.CommunityId == request.CommunityId && m.MemberId == profileId.Value && m.Status == CommunityMembershipStatus.Active, cancellationToken));
            if (!isMember)
            {
                return Result<Paged<CommunityMemberDto>>.Failure(SecurityStatusCodes.UnAuthorized);
            }
        }

        var page = request.Page > 0 ? request.Page : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 20;
        if (pageSize > 100) pageSize = 100;

        var baseQuery = _dbContext.CommunityMemberships
            .AsNoTracking()
            .Where(m => m.CommunityId == request.CommunityId && m.Status == CommunityMembershipStatus.Active);

        if (request.Role.HasValue)
        {
            baseQuery = baseQuery.Where(m => m.Role == request.Role.Value);
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var rawMembers = await baseQuery
            .OrderBy(m => m.Role)
            .ThenBy(m => m.JoinedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new
            {
                m.Id,
                m.MemberId,
                m.Role,
                m.Status,
                m.JoinedDate,
                MemberFullName = m.Member.FullName,
                MemberSpecialization = m.Member.Specialization,
                MemberAvatarKey = m.Member.ProfilePictureObjectKey
            })
            .ToListAsync(cancellationToken);

        var distinctKeys = rawMembers
            .Select(m => m.MemberAvatarKey)
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

        var items = rawMembers.Select(m => new CommunityMemberDto(
            MembershipId: m.Id,
            Member: new ProfileSnapshotDto(
                m.MemberId,
                m.MemberFullName,
                m.MemberSpecialization,
                !string.IsNullOrWhiteSpace(m.MemberAvatarKey) && urlMap.TryGetValue(m.MemberAvatarKey, out var avatarUrl) ? avatarUrl : null
            ),
            Role: m.Role,
            Status: m.Status,
            JoinedDate: m.JoinedDate
        )).ToList();

        var pagedResult = new Paged<CommunityMemberDto>(
            items,
            totalCount,
            pageSize,
            page
        );

        return Result<Paged<CommunityMemberDto>>.Success(pagedResult, OperationStatusCode.Success);
    }
}
