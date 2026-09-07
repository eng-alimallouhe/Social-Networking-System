using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.Discussions.Problems.Problems.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Search.Discussions.Problems.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Search.Discussions.Problems.Queries.GetProblemsSearch;

/// <summary>
/// Handles the execution of <see cref="GetProblemsSearchQuery"/> to search discussion problems and return authoritative problem summaries.
/// </summary>
public class GetProblemsSearchQueryHandler
: IQueryHandler<GetProblemsSearchQuery, SearchResult<ProblemSummaryDto>>
{
    private readonly IProblemSearchService _problemSearchService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;

    public GetProblemsSearchQueryHandler(
        IProblemSearchService problemSearchService,
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService)
    {
        _problemSearchService = problemSearchService;
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<SearchResult<ProblemSummaryDto>>> Handle(
        GetProblemsSearchQuery request,
        CancellationToken cancellationToken)
    {
        var searchResult = await _problemSearchService.SearchProblemsAsync(request, cancellationToken);
        if (!searchResult.Hits.Any())
        {
            return Result<SearchResult<ProblemSummaryDto>>.Success(new SearchResult<ProblemSummaryDto>
            {
                Hits = new List<SearchHit<ProblemSummaryDto>>(),
                Total = searchResult.Total
            }, OperationStatusCode.Success);
        }

        var currentProfileId = _currentUserService.ProfileId;
        var problemIds = searchResult.Hits.Select(h => h.Document.Id).ToList();

        var rawProblems = await _dbContext.Problems
            .AsNoTracking()
            .Where(p => problemIds.Contains(p.Id))
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Status,
                p.Level,
                AuthorId = p.Author.Id,
                AuthorFullName = p.Author.FullName,
                AuthorSpecialization = p.Author.Specialization,
                AuthorProfilePictureObjectKey = p.Author.ProfilePictureObjectKey,
                CommunityId = p.CommunityId,
                CommunityName = p.Community != null ? p.Community.Name : null,
                CommunityType = p.Community != null ? (CommunityType?)p.Community.Type : null,
                CommunityLogoObjectKey = p.Community != null ? p.Community.LogoObjectKey : null,
                UpvotesCount = p.Votes.Count(v => v.Type == VoteType.Upvote),
                DownvotesCount = p.Votes.Count(v => v.Type == VoteType.Downvote),
                SolutionsCount = p.Solutions.Count(s => s.IsActive),
                IsUpVotedByCurrentUser = currentProfileId.HasValue && p.Votes.Any(v => v.VoterId == currentProfileId.Value && v.Type == VoteType.Upvote),
                IsDownVotedByCurrentUser = currentProfileId.HasValue && p.Votes.Any(v => v.VoterId == currentProfileId.Value && v.Type == VoteType.Downvote),
                Tags = p.ProblemTags.Select(pt => pt.Tag.Name).ToList(),
                Topics = p.ProblemTopics.Select(pt => pt.Topic.Name).ToList(),
                p.CreatedAt,
                ContentBlocks = p.ContentBlocks
                    .OrderBy(cb => cb.Order)
                    .Select(cb => new
                    {
                        cb.Id,
                        cb.Type,
                        cb.Content,
                        cb.ExtraInfo,
                        cb.Order
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var imageKeys = rawProblems
            .SelectMany(p => p.ContentBlocks)
            .Where(cb => (cb.Type == ProblemBlockType.Image || cb.Type == ProblemBlockType.Video) && !string.IsNullOrWhiteSpace(cb.Content))
            .Select(cb => cb.Content);

        var distinctKeys = rawProblems
            .Select(p => p.AuthorProfilePictureObjectKey)
            .Concat(rawProblems.Select(p => p.CommunityLogoObjectKey))
            .Concat(imageKeys)
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

        var problems = rawProblems.Select(p => new ProblemSummaryDto(
            Id: p.Id,
            Title: p.Title,
            Status: p.Status,
            Level: p.Level,
            Author: new ProfileSnapshotDto(
                Id: p.AuthorId,
                FullName: p.AuthorFullName,
                Specialization: p.AuthorSpecialization,
                ProfilePictureUrl: p.AuthorProfilePictureObjectKey != null && urlMap.TryGetValue(p.AuthorProfilePictureObjectKey, out var authorPicUrl) ? authorPicUrl : null
            ),
            Community: p.CommunityId.HasValue && p.CommunityName != null && p.CommunityType.HasValue
                ? new CommunitySnapshotDto(
                    Id: p.CommunityId.Value,
                    Name: p.CommunityName,
                    Type: p.CommunityType.Value,
                    LogoUrl: p.CommunityLogoObjectKey != null && urlMap.TryGetValue(p.CommunityLogoObjectKey, out var commLogoUrl) ? commLogoUrl : null
                )
                : null,
            UpvotesCount: p.UpvotesCount,
            DownvotesCount: p.DownvotesCount,
            SolutionsCount: p.SolutionsCount,
            IsUpVotedByCurrentUser: p.IsUpVotedByCurrentUser,
            IsDownVotedByCurrentUser: p.IsDownVotedByCurrentUser,
            Tags: p.Tags,
            Topics: p.Topics,
            CreatedAt: p.CreatedAt,
            ContentBlocks: p.ContentBlocks.Select(cb => new ProblemContentBlockDto(
                Id: cb.Id,
                Type: cb.Type,
                Content: (cb.Type == ProblemBlockType.Image || cb.Type == ProblemBlockType.Video) && !string.IsNullOrWhiteSpace(cb.Content) && urlMap.TryGetValue(cb.Content, out var mediaUrl)
                    ? mediaUrl
                    : cb.Content,
                ExtraInfo: cb.ExtraInfo,
                Order: cb.Order
            )).ToList()
        )).ToList();

        var orderedHits = searchResult.Hits
            .Select(hit =>
            {
                var problemDto = problems.FirstOrDefault(p => p.Id == hit.Document.Id);
                return problemDto != null ? new SearchHit<ProblemSummaryDto>(problemDto, hit.Score) : null;
            })
            .Where(h => h != null)
            .Select(h => h!)
            .ToList();

        return Result<SearchResult<ProblemSummaryDto>>.Success(new SearchResult<ProblemSummaryDto>
        {
            Hits = orderedHits,
            Total = searchResult.Total
        }, OperationStatusCode.Success);
    }
}
