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
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Problems.Problems.Queries.GetProblemsByCommunity;

/// <summary>
/// Query to retrieve a paged list of discussion problems associated with a specific community.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
/// <param name="PageSize">The maximum number of items per page.</param>
/// <param name="CurrentPage">The current page index (1-based).</param>
/// <param name="SearchTerm">Optional search keyword.</param>
public sealed record GetProblemsByCommunityQuery(
    Guid CommunityId,
    int PageSize = 10,
    int CurrentPage = 1,
    string? SearchTerm = null
) : IQuery<Paged<ProblemSummaryDto>>;

/// <summary>
/// Handles <see cref="GetProblemsByCommunityQuery"/> to fetch problems within a specific community.
/// </summary>
internal sealed class GetProblemsByCommunityQueryHandler : IQueryHandler<GetProblemsByCommunityQuery, Paged<ProblemSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetProblemsByCommunityQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<ProblemSummaryDto>>> Handle(GetProblemsByCommunityQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;

        var community = await _dbContext.Communities
            .AsNoTracking()
            .Where(c => c.Id == request.CommunityId && c.IsActive)
            .Select(c => new { c.Id, c.Type, c.OwnerId })
            .FirstOrDefaultAsync(cancellationToken);

        if (community == null)
        {
            return Result<Paged<ProblemSummaryDto>>.Failure(ResourceStatusCode.NotFound);
        }

        if (community.Type == CommunityType.Private)
        {
            var isMember = currentProfileId.HasValue && (community.OwnerId == currentProfileId.Value || await _dbContext.CommunityMemberships.AnyAsync(m => m.CommunityId == request.CommunityId && m.MemberId == currentProfileId.Value && m.Status == CommunityMembershipStatus.Active, cancellationToken));
            if (!isMember)
            {
                return Result<Paged<ProblemSummaryDto>>.Failure(SecurityStatusCodes.UnAuthorized);
            }
        }

        var baseQuery = _dbContext.Problems
            .AsNoTracking()
            .Where(p => p.CommunityId == request.CommunityId && p.IsActive);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim().ToLower();
            baseQuery = baseQuery.Where(p => p.Title.ToLower().Contains(search));
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var problems = await baseQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
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
            pageSize: request.PageSize,
            currentPage: request.CurrentPage), OperationStatusCode.Success);
    }
}
