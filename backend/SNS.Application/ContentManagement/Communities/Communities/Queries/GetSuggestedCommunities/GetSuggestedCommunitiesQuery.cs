using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.ContentManagement.Communities.Communities.Queries.GetSuggestedCommunities;

/// <summary>
/// Represents a query to retrieve suggested/recommended communities for the current authenticated user.
/// </summary>
/// <param name="Count">The maximum number of suggested communities to return.</param>
public sealed record GetSuggestedCommunitiesQuery(int Count = 5) : IQuery<List<CommunitySummaryDto>>;

internal sealed class GetSuggestedCommunitiesQueryHandler : IQueryHandler<GetSuggestedCommunitiesQuery, List<CommunitySummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetSuggestedCommunitiesQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<List<CommunitySummaryDto>>> Handle(GetSuggestedCommunitiesQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        var limit = request.Count > 0 && request.Count <= 20 ? request.Count : 5;

        var baseQuery = _dbContext.Communities
            .AsNoTracking()
            .Where(c => c.IsActive && c.Type == CommunityType.Public);

        if (profileId.HasValue)
        {
            baseQuery = baseQuery.Where(c =>
                c.OwnerId != profileId.Value &&
                !c.Memberships.Any(m => m.MemberId == profileId.Value && m.Status == CommunityMembershipStatus.Active));
        }

        var rawList = await baseQuery
            .OrderByDescending(c => c.Memberships.Count(m => m.Status == CommunityMembershipStatus.Active))
            .ThenByDescending(c => c.CreatedAt)
            .Take(limit)
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

        return Result<List<CommunitySummaryDto>>.Success(items, OperationStatusCode.Success);
    }
}
