using SNS.Application.Search.ContentManagement.Communitites.Abstractions;
using SNS.Application.Search.ContentManagement.Communitites.Queries;
using SNS.Application.Search.Identity.Users.Abstractions;
using SNS.Application.Search.Identity.Users.Queries;
using SNS.Application.Search.Jobs.Abstractions;
using SNS.Application.Search.Jobs.Queries.GetJobsSearch;
using SNS.Application.Search.Profiles.Profiles.Abstractions;
using SNS.Application.Search.Profiles.Profiles.Queries.GetProfilesSearch;
using SNS.Application.Search.Projects.Abstractions;
using SNS.Application.Search.Projects.Queries.GetProjectsSearch;
using SNS.Application.Search.Queries.GetGlobalSearch;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Search.Queries.GlobalSearch;

public class GetGlobalSearchQueryHandler
: IQueryHandler<GetGlobalSearchQuery, GlobalSearchResultDto>
{
    private readonly IUserSearchService _userSearch;
    private readonly IProfileSearchService _profileSearch;
    private readonly IProjectSearchService _projectSearch;
    private readonly ICommunitySearchService _communitySearch;
    private readonly IJobSearchService _jobSearch;

    public GetGlobalSearchQueryHandler(
        IUserSearchService userSearch,
        IProfileSearchService profileSearch,
        IProjectSearchService projectSearch,
        ICommunitySearchService communitySearch,
        IJobSearchService jobSearch)
    {
        _userSearch = userSearch;
        _profileSearch = profileSearch;
        _projectSearch = projectSearch;
        _communitySearch = communitySearch;
        _jobSearch = jobSearch;
    }

    public async Task<Result<GlobalSearchResultDto>> Handle(
        GetGlobalSearchQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return Result<GlobalSearchResultDto>.Success(new GlobalSearchResultDto(), OperationStatusCode.Success);
        }

        var userQuery = new UserSearchQuery(SearchTerm: request.SearchTerm, PageSize: request.TopResultsPerCategory);
        var profileQuery = new ProfileSearchQuery { SearchTerm = request.SearchTerm, PageSize = request.TopResultsPerCategory };
        var projectQuery = new ProjectSearchQuery(SearchTerm: request.SearchTerm, PageSize: request.TopResultsPerCategory);
        var jobQuery = new JobSearchQuery(SearchTerm: request.SearchTerm, PageSize: request.TopResultsPerCategory);
        var communityQuery = new CommunitySearchQuery(SearchTerm: request.SearchTerm, PageSize: request.TopResultsPerCategory);

        var profilesTask = _profileSearch.SearchProfilesAsync(profileQuery, cancellationToken);
        var projectsTask = _projectSearch.SearchProjectsAsync(projectQuery, cancellationToken);
        var communitiesTask = _communitySearch.SearchCommunitiesAsync(communityQuery, cancellationToken);
        var jobsTask = _jobSearch.SearchJobsAsync(jobQuery, cancellationToken);

        await Task.WhenAll(profilesTask, projectsTask, communitiesTask, jobsTask);

        var result = new GlobalSearchResultDto
        {
            Profiles = profilesTask.Result.Documents,
            Projects = projectsTask.Result.Documents,
            Communities = communitiesTask.Result.Documents,
            Jobs = jobsTask.Result.Documents
        };

        return Result<GlobalSearchResultDto>.Success(result, OperationStatusCode.Success);
    }
}
