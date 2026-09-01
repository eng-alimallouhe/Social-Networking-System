using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Projects.Abstractions;
using SNS.Application.Projects.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Projects.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Projects.Queries.GetProjectFeed;

/// <summary>
/// Handles the execution of <see cref="GetProjectFeedQuery"/> to retrieve a personalized project feed.
/// </summary>
internal sealed class GetProjectFeedQueryHandler
    : IQueryHandler<GetProjectFeedQuery, List<ProjectOverviewDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IProjectCacheService _projectCacheService;
    private readonly IProjectFeedService _projectFeedService;

    public GetProjectFeedQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IProjectCacheService projectCacheService,
        IProjectFeedService projectFeedService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _projectCacheService = projectCacheService;
        _projectFeedService = projectFeedService;
    }

    public async Task<Result<List<ProjectOverviewDto>>> Handle(GetProjectFeedQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<List<ProjectOverviewDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        int page = request.CurrentPage <= 0 ? 1 : request.CurrentPage;
        int pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        long start = (long)(page - 1) * pageSize;
        long stop = start + pageSize - 1;

        var cachedFeed = await _projectCacheService.GetProfileFeedAsync(profileId.Value, start, stop, cancellationToken);
        if (cachedFeed.Any())
        {
            var projectIds = cachedFeed.Select(f => f.ProjectId).ToList();
            return await ProjectAndOrderFeedAsync(projectIds, cancellationToken);
        }

        var feedParams = await _dbContext.Profiles
            .Where(p => p.Id == profileId.Value && p.IsActive)
            .Select(p => new ProjectFeedParameter(
                p.Id,
                p.ProfileSkills.Select(ps => ps.Skill.Name).ToList(),
                p.ProfileTags.Select(pt => new ProjectTagSnapshot(pt.TagId, pt.Score)).ToList(),
                _dbContext.ProjectViews.Where(pv => pv.ViewerId == profileId.Value && pv.IsActive).Select(pv => pv.ProjectId).ToList(),
                _dbContext.Blocks
                    .Where(b => b.BlockerId == profileId.Value || b.BlockedId == profileId.Value)
                    .Select(b => b.BlockerId == profileId.Value ? b.BlockedId : b.BlockerId)
                    .ToList(),
                p.Followings.Select(f => f.FollowingId).ToList(),
                300
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (feedParams == null)
        {
            return Result<List<ProjectOverviewDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        await _projectFeedService.GenerateAndCacheUserFeedAsync(profileId.Value, feedParams, cancellationToken);

        var newCachedFeed = await _projectCacheService.GetProfileFeedAsync(profileId.Value, start, stop, cancellationToken);
        if (!newCachedFeed.Any())
        {
            return Result<List<ProjectOverviewDto>>.Success(new List<ProjectOverviewDto>(), OperationStatusCode.Success);
        }

        var newProjectIds = newCachedFeed.Select(f => f.ProjectId).ToList();
        return await ProjectAndOrderFeedAsync(newProjectIds, cancellationToken);
    }

    private async Task<Result<List<ProjectOverviewDto>>> ProjectAndOrderFeedAsync(List<Guid> projectIds, CancellationToken cancellationToken)
    {
        if (!projectIds.Any())
        {
            return Result<List<ProjectOverviewDto>>.Success(new List<ProjectOverviewDto>(), OperationStatusCode.Success);
        }

        var projects = await _dbContext.Projects
            .Where(p => projectIds.Contains(p.Id))
            .Select(p => new ProjectOverviewDto(
                p.Id,
                p.Title,
                p.ShortDescription,
                p.Type,
                p.Status,
                p.Contributors.Count(c => c.InvitingStatus == InvitingStatus.Accepted),
                p.Contributors
                    .Where(c => c.InvitingStatus == InvitingStatus.Accepted)
                    .Take(3)
                    .Select(c => new ProjectParticipantDto(
                        c.ContributorId,
                        c.Contributor.ProfilePictureObjectKey
                    ))
                    .ToList(),
                p.Skills.Count(),
                p.Skills
                    .Take(3)
                    .Select(s => new ProjectSkillDto(
                        s.SkillId,
                        s.Skill.Name
                    ))
                    .ToList(),
                p.Ratings.Count(),
                p.Ratings.Select(r => (double?)r.RatingValue).Average() ?? 0.0,
                p.GitHubUrl,
                p.LiveDemoUrl
            ))
            .ToListAsync(cancellationToken);

        var orderedProjects = projectIds
            .Join(
                projects,
                id => id,
                p => p.ProjectId,
                (_, project) => project)
            .ToList();

        return Result<List<ProjectOverviewDto>>.Success(orderedProjects, ResourceStatusCode.Found);
    }
}
