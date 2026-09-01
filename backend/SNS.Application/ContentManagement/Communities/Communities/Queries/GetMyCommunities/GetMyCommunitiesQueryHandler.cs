using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Communities.Communities.Queries.GetMyCommunities;

/// <summary>
/// Handles the execution of <see cref="GetMyCommunitiesQuery"/> to retrieve paginated communities for the current authenticated user.
/// </summary>
internal sealed class GetMyCommunitiesQueryHandler : IQueryHandler<GetMyCommunitiesQuery, Paged<CommunitySummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetMyCommunitiesQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<CommunitySummaryDto>>> Handle(GetMyCommunitiesQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<Paged<CommunitySummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var page = request.Page > 0 ? request.Page : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 10;
        if (pageSize > 50) pageSize = 50;

        var baseQuery = _dbContext.Communities
            .AsNoTracking()
            .Where(c => c.IsActive &&
                (c.OwnerId == profileId.Value ||
                 c.Memberships.Any(m => m.MemberId == profileId.Value && m.Status == CommunityMembershipStatus.Active)));

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var rawList = await baseQuery
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.Type,
                c.LogoObjectKey,
                MembersCount = c.Memberships.Count(m => m.Status == CommunityMembershipStatus.Active),
                c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var distinctKeys = rawList
            .Select(c => c.LogoObjectKey)
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

        var items = rawList.Select(c => new CommunitySummaryDto(
            c.Id,
            c.Name,
            c.Description,
            c.Type,
            !string.IsNullOrWhiteSpace(c.LogoObjectKey) && urlMap.TryGetValue(c.LogoObjectKey, out var url) ? url : null,
            c.MembersCount,
            c.CreatedAt
        )).ToList();

        var pagedResult = new Paged<CommunitySummaryDto>(
            items,
            totalCount,
            pageSize,
            page
        );

        return Result<Paged<CommunitySummaryDto>>.Success(pagedResult, OperationStatusCode.Success);
    }
}
