using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Search.Profiles.Profiles.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Search.Profiles.Profiles.Queries.GetProfilesSearch;

/// <summary>
/// Handles the execution of <see cref="GetProfilesSearchQuery"/> to search user profiles and return authoritative profile summaries.
/// </summary>
public class GetProfilesSearchQueryHandler
: IQueryHandler<GetProfilesSearchQuery, SearchResult<ProfileSummaryDto>>
{
    private readonly IProfileSearchService _profileSearchService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetProfilesSearchQueryHandler(
        IProfileSearchService profileSearchService,
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _profileSearchService = profileSearchService;
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<SearchResult<ProfileSummaryDto>>> Handle(
        GetProfilesSearchQuery request,
        CancellationToken cancellationToken)
    {
        var searchResult = await _profileSearchService.SearchProfilesAsync(request, cancellationToken);
        if (!searchResult.Hits.Any())
        {
            return Result<SearchResult<ProfileSummaryDto>>.Success(new SearchResult<ProfileSummaryDto>
            {
                Hits = new List<SearchHit<ProfileSummaryDto>>(),
                Total = searchResult.Total
            }, OperationStatusCode.Success);
        }

        var profileIds = searchResult.Hits.Select(h => h.Document.Id).ToList();
        var currentProfileId = _currentUserService.ProfileId;

        var profiles = await _dbContext.Profiles
            .Where(p => profileIds.Contains(p.Id))
            .Select(p => new ProfileSummaryDto(
                p.Id,
                p.FullName,
                p.Specialization,
                p.Bio,
                p.ProfilePictureObjectKey,
                p.Followers.Count(),
                p.Followings.Count(),
                p.ProfileSkills.Select(ps => ps.Skill.Name).ToList(),
                p.CreatedAt,
                currentProfileId != null && p.Followers.Any(f => f.FollowerId == currentProfileId.Value),
                currentProfileId != null && _dbContext.Blocks.Any(b => b.BlockerId == currentProfileId.Value && b.BlockedId == p.Id)
            ))
            .ToListAsync(cancellationToken);

        var orderedHits = searchResult.Hits
            .Select(hit =>
            {
                var profileDto = profiles.FirstOrDefault(p => p.Id == hit.Document.Id);
                return profileDto != null ? new SearchHit<ProfileSummaryDto>(profileDto, hit.Score) : null;
            })
            .Where(h => h != null)
            .Select(h => h!)
            .ToList();

        return Result<SearchResult<ProfileSummaryDto>>.Success(new SearchResult<ProfileSummaryDto>
        {
            Hits = orderedHits,
            Total = searchResult.Total
        }, OperationStatusCode.Success);
    }
}
