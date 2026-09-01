using Microsoft.EntityFrameworkCore;
using SNS.Application.Projects.Abstractions;
using SNS.Application.Projects.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Projects.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Projects.Services;

public sealed class ProjectFeedService : IProjectFeedService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IProjectCacheService _projectCacheService;

    public ProjectFeedService(IApplicationDbContext dbContext, IProjectCacheService projectCacheService)
    {
        _dbContext = dbContext;
        _projectCacheService = projectCacheService;
    }

    public async Task<Result> GenerateAndCacheUserFeedAsync(Guid profileId, ProjectFeedParameter feedParams, CancellationToken cancellationToken = default)
    {
        var followedProfiles = feedParams.FollowedProfilesIds.ToHashSet();
        var excludedProjects = feedParams.ExcludedProjectsIds.ToHashSet();
        var excludedProfiles = feedParams.ExcludedProfilesIds.ToHashSet();
        var userSkills = feedParams.Skills.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var userTagsDict = feedParams.Tags.ToDictionary(x => x.TagId, x => x.Score);

        var rawCandidates = await _dbContext.Projects
            .Where(p =>
                p.IsActive &&
                p.Status != ProjectStatus.Draft &&
                !excludedProjects.Contains(p.Id) &&
                !excludedProfiles.Contains(p.OwnerId)
            )
            .OrderByDescending(p => p.CreatedAt)
            .Take(300)
            .Select(p => new
            {
                Id = p.Id,
                CreatedAt = p.CreatedAt,
                OwnerId = p.OwnerId,
                Tags = p.Tags.Select(pt => new
                {
                    TagId = pt.TagId
                }),
                Skills = p.Skills.Select(ps => ps.Skill.Name),
                ViewsCount = p.Views.Count(),
                RatingsCount = p.Ratings.Count()
            })
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var rankedItems = new List<ProjectFeedItemModel>();

        foreach (var project in rawCandidates)
        {
            double hoursOld = (now - project.CreatedAt).TotalHours;
            double timeScore = 1.0 / Math.Pow(hoursOld + 2, 1.5);

            double engagementScore = (project.RatingsCount * 3.0) + (project.ViewsCount * 0.2);

            double followBonus = followedProfiles.Contains(project.OwnerId) ? 20.0 : 0.0;

            double tagInterestBonus = 0;
            foreach (var projectTag in project.Tags)
            {
                if (userTagsDict.TryGetValue(projectTag.TagId, out double userTagScore))
                {
                    tagInterestBonus += userTagScore * 10.0;
                }
            }

            double skillInterestBonus = 0;
            foreach (var projectSkill in project.Skills)
            {
                if (userSkills.Contains(projectSkill))
                {
                    skillInterestBonus += 15.0; 
                }
            }

            double finalScore = (engagementScore * timeScore) + followBonus + tagInterestBonus + skillInterestBonus;
            
            rankedItems.Add(new ProjectFeedItemModel(project.Id, finalScore));
        }

        var topSortedFeed = rankedItems
            .OrderByDescending(x => x.Score)
            .Take(100)
            .ToList();

        if (topSortedFeed.Any())
        {
            await _projectCacheService.SetProfileFeedAsync(profileId, topSortedFeed, cancellationToken);
        }

        return Result.Success(OperationStatusCode.Success);
    }
}
