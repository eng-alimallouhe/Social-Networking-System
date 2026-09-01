using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.ContentManagement.Posts.PostMentions.Contracts;
using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Projects.Contracts;
using SNS.Application.Search.ContentManagement.Communitites.Abstractions;
using SNS.Application.Search.ContentManagement.Communitites.Queries.GetCommunitiesSearch;
using SNS.Application.Search.ContentManagement.Posts.Abstractions;
using SNS.Application.Search.ContentManagement.Posts.Queries.GetPostsSearch;
using SNS.Application.Discussions.Problems.Problems.Contracts;
using SNS.Application.Search.Discussions.Problems.Abstractions;
using SNS.Application.Search.Discussions.Problems.Queries.GetProblemsSearch;
using SNS.Application.Search.Jobs.Abstractions;
using SNS.Application.Search.Jobs.Contracts;
using SNS.Application.Search.Jobs.Queries.GetJobsSearch;
using SNS.Application.Search.Profiles.Profiles.Abstractions;
using SNS.Application.Search.Profiles.Profiles.Queries.GetProfilesSearch;
using SNS.Application.Search.Projects.Abstractions;
using SNS.Application.Search.Projects.Queries.GetProjectsSearch;
using SNS.Application.Search.Queries.GetGlobalSearch;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Domain.Projects.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Search.Queries.GlobalSearch;

/// <summary>
/// Handles the execution of <see cref="GetGlobalSearchQuery"/> to perform cross-category global search.
/// </summary>
public class GetGlobalSearchQueryHandler
: IQueryHandler<GetGlobalSearchQuery, GlobalSearchResultDto>
{
    private readonly IProfileSearchService _profileSearch;
    private readonly IProjectSearchService _projectSearch;
    private readonly ICommunitySearchService _communitySearch;
    private readonly IJobSearchService _jobSearch;
    private readonly IProblemSearchService _problemSearch;
    private readonly IPostSearchService _postSearch;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetGlobalSearchQueryHandler(
        IProfileSearchService profileSearch,
        IProjectSearchService projectSearch,
        ICommunitySearchService communitySearch,
        IJobSearchService jobSearch,
        IProblemSearchService problemSearch,
        IPostSearchService postSearch,
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _profileSearch = profileSearch;
        _projectSearch = projectSearch;
        _communitySearch = communitySearch;
        _jobSearch = jobSearch;
        _problemSearch = problemSearch;
        _postSearch = postSearch;
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<GlobalSearchResultDto>> Handle(
        GetGlobalSearchQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return Result<GlobalSearchResultDto>.Success(new GlobalSearchResultDto(), OperationStatusCode.Success);
        }

        var profileQuery = new GetProfilesSearchQuery(SearchTerm: request.SearchTerm, PageSize: request.TopResultsPerCategory);
        var projectQuery = new GetProjectsSearchQuery(SearchTerm: request.SearchTerm, PageSize: request.TopResultsPerCategory);
        var jobQuery = new GetJobsSearchQuery(SearchTerm: request.SearchTerm, PageSize: request.TopResultsPerCategory);
        var communityQuery = new GetCommunitiesSearchQuery(SearchTerm: request.SearchTerm, PageSize: request.TopResultsPerCategory);
        var problemQuery = new GetProblemsSearchQuery(SearchTerm: request.SearchTerm, PageSize: request.TopResultsPerCategory);
        var postQuery = new GetPostsSearchQuery(SearchTerm: request.SearchTerm, PageSize: request.TopResultsPerCategory);

        var profilesTask = _profileSearch.SearchProfilesAsync(profileQuery, cancellationToken);
        var projectsTask = _projectSearch.SearchProjectsAsync(projectQuery, cancellationToken);
        var jobsTask = _jobSearch.SearchJobsAsync(jobQuery, cancellationToken);
        var communitiesTask = _communitySearch.SearchCommunitiesAsync(communityQuery, cancellationToken);
        var problemsTask = _problemSearch.SearchProblemsAsync(problemQuery, cancellationToken);
        var postsTask = _postSearch.SearchAsync(postQuery, cancellationToken);

        await Task.WhenAll(
            profilesTask,
            projectsTask,
            jobsTask,
            communitiesTask,
            problemsTask,
            postsTask
        );

        // 1. Profiles
        var currentProfileId = _currentUserService.ProfileId;
        var profileIds = profilesTask.Result.Hits.Select(h => h.Document.Id).ToList();
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
        var orderedProfiles = profileIds.Select(id => profiles.FirstOrDefault(p => p.Id == id)).Where(p => p != null).Select(p => p!).ToList();

        // 2. Projects
        var projectIds = projectsTask.Result.Hits.Select(h => h.Document.Id).ToList();
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
        var orderedProjects = projectIds.Select(id => projects.FirstOrDefault(p => p.ProjectId == id)).Where(p => p != null).Select(p => p!).ToList();

        // 3. Communities
        var communityIds = communitiesTask.Result.Hits.Select(h => h.Document.Id).ToList();
        var communities = await _dbContext.Communities
            .Where(c => communityIds.Contains(c.Id))
            .Select(c => new CommunitySummaryDto(
                c.Id,
                c.Name,
                c.Description,
                c.Type,
                c.LogoObjectKey,
                c.Memberships.Count,
                c.CreatedAt
            ))
            .ToListAsync(cancellationToken);
        var orderedCommunities = communityIds.Select(id => communities.FirstOrDefault(c => c.Id == id)).Where(c => c != null).Select(c => c!).ToList();

        // 4. Jobs
        var jobIds = jobsTask.Result.Hits.Select(h => h.Document.Id).ToList();
        var jobs = await _dbContext.Jobs
            .Where(j => jobIds.Contains(j.Id))
            .Select(j => new JobSummaryDto(
                j.Id,
                j.Title,
                j.Description,
                j.Location,
                j.Type,
                j.MinSalary,
                j.MaxSalary,
                j.CurrencyCode,
                j.SalaryType,
                j.Company.Name,
                j.CreatedAt,
                j.ClosedAt
            ))
            .ToListAsync(cancellationToken);
        var orderedJobs = jobIds.Select(id => jobs.FirstOrDefault(j => j.Id == id)).Where(j => j != null).Select(j => j!).ToList();

        // 5. Problems
        var problemIds = problemsTask.Result.Hits.Select(h => h.Document.Id).ToList();
        var problems = await _dbContext.Problems
            .Where(p => problemIds.Contains(p.Id))
            .Select(p => new ProblemSummaryDto(
                p.Id,
                p.Title,
                p.Status,
                p.Level,
                p.AuthorId,
                p.Author.FullName,
                p.Author.ProfilePictureObjectKey,
                p.Votes.Count(v => v.Type == VoteType.Upvote),
                p.Solutions.Count(),
                p.ProblemTags.Select(pt => pt.Tag.Name).ToList(),
                p.ProblemTopics.Select(pt => pt.Topic.Name).ToList(),
                p.CreatedAt
            ))
            .ToListAsync(cancellationToken);
        var orderedProblems = problemIds.Select(id => problems.FirstOrDefault(p => p.Id == id)).Where(p => p != null).Select(p => p!).ToList();

        // 6. Posts
        var postIds = postsTask.Result.Hits.Select(h => h.Document.Id).ToList();

        var rawPosts = await _dbContext.Posts
            .AsNoTracking()
            .Where(p => postIds.Contains(p.Id))
            .Select(p => new
            {
                p.Id,
                AuthorId = p.Author.Id,
                AuthorFullName = p.Author.FullName,
                AuthorSpecialization = p.Author.Specialization,
                AuthorProfilePictureKey = p.Author.ProfilePictureObjectKey,

                p.CommunityId,
                CommunityType = p.Community != null ? p.Community.Type : (SNS.Domain.ContentManagement.Communities.Enums.CommunityType?)null,
                CommunityName = p.Community != null ? p.Community.Name : null,
                CommunityLogoKey = p.Community != null ? p.Community.LogoObjectKey : null,

                p.Title,
                p.Content,
                p.CreatedAt,
                p.UpdatedAt,
                p.LastInteractedAt,
                Media = p.Media.OrderBy(m => m.Order).Select(m => new { m.ObjectKey, m.Order, m.Type }).ToList(),
                Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList(),
                CommentsCount = p.Comments.Count(c => c.IsActive),
                ReactionsCount = p.Reactions.Count(),
                ViewsCount = p.Views.Count(),
                SavesCount = p.SavedPosts.Count(),
                CurrentUserReaction = currentProfileId.HasValue ? p.Reactions.Where(r => r.ReactorId == currentProfileId.Value).Select(r => (ReactionType?)r.Type).FirstOrDefault() : null,

                Mentions = p.Mentions
                    .Where(m => m.Profile.IsActive)
                    .Select(m => new
                    {
                        m.ProfileId,
                        DisplayName = m.Profile.FullName,
                        ProfilePictureKey = m.Profile.ProfilePictureObjectKey
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var distinctPostKeys = rawPosts
            .Select(p => p.AuthorProfilePictureKey)
            .Concat(rawPosts.Select(p => p.CommunityLogoKey))
            .Concat(rawPosts.SelectMany(p => p.Media.Select(m => m.ObjectKey)))
            .Concat(rawPosts.SelectMany(p => p.Mentions.Select(m => m.ProfilePictureKey)))
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct()
            .ToList();

        var postUrlTasks = distinctPostKeys.Select(async k => new
        {
            Key = k!,
            Url = await _fileStorageService.GetTemporaryUrlAsync(k!, TimeSpan.FromHours(1))
        });
        var resolvedPostUrls = await Task.WhenAll(postUrlTasks);
        var postUrlMap = resolvedPostUrls.ToDictionary(r => r.Key, r => r.Url);

        var posts = rawPosts.Select(p => new PostOverviewDto(
            Id: p.Id,
            Author: new ProfileSnapshotDto(
                p.AuthorId,
                p.AuthorFullName,
                p.AuthorSpecialization,
                p.AuthorProfilePictureKey != null && postUrlMap.TryGetValue(p.AuthorProfilePictureKey, out var authorPicUrl) ? authorPicUrl : null
            ),
            Community: p.CommunityId.HasValue && p.CommunityType.HasValue && p.CommunityName != null
                ? new CommunitySnapshotDto(
                    p.CommunityId.Value,
                    p.CommunityName,
                    p.CommunityType.Value,
                    p.CommunityLogoKey != null && postUrlMap.TryGetValue(p.CommunityLogoKey, out var commLogoUrl) ? commLogoUrl : null)
                : null,
            Title: p.Title,
            Content: p.Content,
            CreatedAt: p.CreatedAt,
            UpdatedAt: p.UpdatedAt,
            LastInteractedAt: p.LastInteractedAt,
            Media: p.Media.Select(m => new PostMediaDto(
                Url: postUrlMap.TryGetValue(m.ObjectKey, out var mediaUrl) ? mediaUrl : m.ObjectKey,
                Order: m.Order,
                Type: m.Type
            )).ToList(),
            Tags: p.Tags,
            CommentsCount: p.CommentsCount,
            ReactionsCount: p.ReactionsCount,
            ViewsCount: p.ViewsCount,
            SavesCount: p.SavesCount,
            CurrentUserReaction: p.CurrentUserReaction,
            Mentions: p.Mentions.Select(m => new PostMentionDto(
                m.ProfileId,
                m.DisplayName,
                m.ProfilePictureKey != null && postUrlMap.TryGetValue(m.ProfilePictureKey, out var mentionPicUrl) ? mentionPicUrl : null
            )).ToList()
        )).ToList();

        var orderedPosts = postIds.Select(id => posts.FirstOrDefault(p => p.Id == id)).Where(p => p != null).Select(p => p!).ToList();

        var result = new GlobalSearchResultDto
        {
            Profiles = orderedProfiles,
            Projects = orderedProjects,
            Communities = orderedCommunities,
            Jobs = orderedJobs,
            Problems = orderedProblems,
            Posts = orderedPosts
        };

        return Result<GlobalSearchResultDto>.Success(result, OperationStatusCode.Success);
    }
}
