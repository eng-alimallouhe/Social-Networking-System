using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.Discussions.Problems.Problems.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Discussions.Problems.Problems.Queries.GetProblems;

/// <summary>
/// Query to retrieve a paged list of discussion problems for the main Problems page,
/// featuring database-level recommendation, relevance ranking, and pagination.
/// </summary>
/// <param name="Page">The page index for pagination (1-based).</param>
/// <param name="PageSize">The maximum number of items per page.</param>
public sealed record GetProblemsQuery(
    int Page = 1,
    int PageSize = 10
) : IQuery<Paged<ProblemSummaryDto>>;

/// <summary>
/// Handles <see cref="GetProblemsQuery"/> by retrieving suitable problems directly from the database,
/// applying exclusions, visibility, relevance ranking, and pagination.
/// </summary>
internal sealed class GetProblemsQueryHandler : IQueryHandler<GetProblemsQuery, Paged<ProblemSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetProblemsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<ProblemSummaryDto>>> Handle(GetProblemsQuery request, CancellationToken cancellationToken)
    {
        int page = request.Page <= 0 ? 1 : request.Page;
        int pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var currentProfileId = _currentUserService.ProfileId;

        // Base query: only active problems from active authors
        var baseQuery = _dbContext.Problems
            .AsNoTracking()
            .Where(p => p.IsActive && p.Author.IsActive);

        // Apply profile-specific visibility and exclusions
        if (currentProfileId.HasValue)
        {
            var myId = currentProfileId.Value;

            // Exclude blocked profiles mutually
            var blockedProfileIds = _dbContext.Blocks
                .Where(b => b.BlockerId == myId || b.BlockedId == myId)
                .Select(b => b.BlockerId == myId ? b.BlockedId : b.BlockerId);

            baseQuery = baseQuery.Where(p => !blockedProfileIds.Contains(p.AuthorId));

            // Community visibility: problem is either outside communities, in a public/restricted community,
            // or in a private community where the current user holds active membership
            var memberCommunityIds = _dbContext.CommunityMemberships
                .Where(cm => cm.MemberId == myId)
                .Select(cm => cm.CommunityId);

            baseQuery = baseQuery.Where(p =>
                !p.CommunityId.HasValue ||
                p.Community!.Type != CommunityType.Private ||
                memberCommunityIds.Contains(p.CommunityId.Value));
        }
        else
        {
            // Guests only see problems from public/restricted communities or outside communities
            baseQuery = baseQuery.Where(p =>
                !p.CommunityId.HasValue ||
                p.Community!.Type != CommunityType.Private);
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        // Apply ranking:
        if (currentProfileId.HasValue)
        {
            var myId = currentProfileId.Value;

            var followedAuthorIds = _dbContext.Follows
                .Where(f => f.FollowerId == myId)
                .Select(f => f.FollowingId);

            var userTopicIds = _dbContext.ProfileTopics
                .Where(pt => pt.ProfileId == myId)
                .Select(pt => pt.TopicId);

            baseQuery = baseQuery
                .OrderByDescending(p =>
                    followedAuthorIds.Contains(p.AuthorId) ? 1 : 0
                )
                .ThenByDescending(p =>
                    p.ProblemTopics.Any(pt => userTopicIds.Contains(pt.TopicId)) ? 1 : 0
                )
                .ThenByDescending(p =>
                    p.Status == ProblemStatus.Open ? 1 : 0
                )
                .ThenByDescending(p =>
                    (p.Votes.Count(v => v.Type == VoteType.Upvote) * 2) +
                    (p.Solutions.Count(s => s.IsActive) * 3)
                )
                .ThenByDescending(p => p.CreatedAt);
        }
        else
        {
            baseQuery = baseQuery
                .OrderByDescending(p =>
                    p.Status == ProblemStatus.Open ? 1 : 0
                )
                .ThenByDescending(p =>
                    (p.Votes.Count(v => v.Type == VoteType.Upvote) * 2) +
                    (p.Solutions.Count(s => s.IsActive) * 3)
                )
                .ThenByDescending(p => p.CreatedAt);
        }

        // Materialize only the requested page slice with direct projection
        var problems = await baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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

        // Resolve URLs in batch
        var imageKeys = problems
            .SelectMany(p => p.ContentBlocks)
            .Where(cb => (cb.Type == ProblemBlockType.Image || cb.Type == ProblemBlockType.Video) && !string.IsNullOrWhiteSpace(cb.Content))
            .Select(cb => cb.Content);

        var distinctKeys = problems
            .Select(p => p.AuthorProfilePictureObjectKey)
            .Concat(problems.Select(p => p.CommunityLogoObjectKey))
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

        // Map DTOs
        var items = problems.Select(p => new ProblemSummaryDto(
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

        return Result<Paged<ProblemSummaryDto>>.Success(new Paged<ProblemSummaryDto>(
            items: items,
            count: totalCount,
            pageSize: pageSize,
            currentPage: page), OperationStatusCode.Success);
    }
}
