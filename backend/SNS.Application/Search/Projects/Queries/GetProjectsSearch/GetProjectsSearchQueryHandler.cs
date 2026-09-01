using Microsoft.EntityFrameworkCore;
using SNS.Application.Projects.Contracts;
using SNS.Application.Search.Projects.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Projects.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Search.Projects.Queries.GetProjectsSearch;

/// <summary>
/// Handles the execution of <see cref="GetProjectsSearchQuery"/> to search projects and return authoritative project overviews.
/// </summary>
public class GetProjectsSearchQueryHandler
: IQueryHandler<GetProjectsSearchQuery, SearchResult<ProjectOverviewDto>>
{
    private readonly IProjectSearchService _projectSearchService;
    private readonly IApplicationDbContext _dbContext;

    public GetProjectsSearchQueryHandler(
        IProjectSearchService projectSearchService,
        IApplicationDbContext dbContext)
    {
        _projectSearchService = projectSearchService;
        _dbContext = dbContext;
    }

    public async Task<Result<SearchResult<ProjectOverviewDto>>> Handle(
        GetProjectsSearchQuery request,
        CancellationToken cancellationToken)
    {
        var searchResult = await _projectSearchService.SearchProjectsAsync(request, cancellationToken);
        if (!searchResult.Hits.Any())
        {
            return Result<SearchResult<ProjectOverviewDto>>.Success(new SearchResult<ProjectOverviewDto>
            {
                Hits = new List<SearchHit<ProjectOverviewDto>>(),
                Total = searchResult.Total
            }, OperationStatusCode.Success);
        }

        var projectIds = searchResult.Hits.Select(h => h.Document.Id).ToList();

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
                    .OrderBy(c => c.Id)
                    .Take(3)
                    .Select(c => new ProjectParticipantDto(
                        c.ContributorId,
                        c.Contributor.ProfilePictureObjectKey
                    ))
                    .ToList(),
                p.Skills.Count(),
                p.Skills
                    .OrderBy(s => s.Id)
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

        var orderedHits = searchResult.Hits
            .Select(hit =>
            {
                var projectDto = projects.FirstOrDefault(p => p.ProjectId == hit.Document.Id);
                return projectDto != null ? new SearchHit<ProjectOverviewDto>(projectDto, hit.Score) : null;
            })
            .Where(h => h != null)
            .Select(h => h!)
            .ToList();

        return Result<SearchResult<ProjectOverviewDto>>.Success(new SearchResult<ProjectOverviewDto>
        {
            Hits = orderedHits,
            Total = searchResult.Total
        }, OperationStatusCode.Success);
    }
}
