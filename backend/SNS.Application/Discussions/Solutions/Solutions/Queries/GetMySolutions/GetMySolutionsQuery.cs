using Microsoft.EntityFrameworkCore;
using SNS.Application.Discussions.Solutions.Solutions.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Domain.Discussions.Solutions.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Solutions.Solutions.Queries.GetMySolutions;

/// <summary>
/// Query to retrieve a paged list of solutions authored by the authenticated user.
/// </summary>
/// <param name="PageSize">The maximum number of solutions per page.</param>
/// <param name="CurrentPage">The current page index (1-based).</param>
public sealed record GetMySolutionsQuery(
    int PageSize = 10,
    int CurrentPage = 1
) : IQuery<Paged<SolutionSummaryDto>>;

/// <summary>
/// Handles <see cref="GetMySolutionsQuery"/> to fetch authored solutions.
/// </summary>
internal sealed class GetMySolutionsQueryHandler : IQueryHandler<GetMySolutionsQuery, Paged<SolutionSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetMySolutionsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<SolutionSummaryDto>>> Handle(GetMySolutionsQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<Paged<SolutionSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var baseQuery = _dbContext.Solutions
            .AsNoTracking()
            .Where(s => s.AuthorId == profileId.Value && s.IsActive);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var solutions = await baseQuery
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new
            {
                s.Id,
                s.ProblemId,
                s.Status,
                s.CreatedAt,
                s.UpdatedAt,
                AuthorId = s.AuthorId,
                AuthorProfile = _dbContext.Profiles
                    .Where(p => p.Id == s.AuthorId)
                    .Select(p => new
                    {
                        p.FullName,
                        p.Specialization,
                        p.ProfilePictureObjectKey
                    })
                    .FirstOrDefault(),
                ContentBlocks = s.ContentBlocks
                    .OrderBy(cb => cb.Order)
                    .Select(cb => new
                    {
                        cb.Id,
                        cb.Type,
                        cb.Content,
                        cb.ExtraInfo,
                        cb.Order
                    })
                    .ToList(),
                UpvotesCount = s.Votes.Count(v => v.Type == VoteType.Upvote),
                DownvotesCount = s.Votes.Count(v => v.Type == VoteType.Downvote),
                DiscussionsCount = s.Discussions.Count(d => d.IsActive),
                CurrentUserVote = s.Votes.Where(v => v.VoterId == profileId.Value).Select(v => (VoteType?)v.Type).FirstOrDefault(),
                IsUpVotedByCurrentUser = s.Votes.Any(v => v.VoterId == profileId.Value && v.Type == VoteType.Upvote),
                IsDownVotedByCurrentUser = s.Votes.Any(v => v.VoterId == profileId.Value && v.Type == VoteType.Downvote)
            })
            .ToListAsync(cancellationToken);

        var imageKeys = solutions
            .SelectMany(s => s.ContentBlocks)
            .Where(cb => cb.Type == SolutionBlockType.Media && !string.IsNullOrWhiteSpace(cb.Content))
            .Select(cb => cb.Content);

        var distinctKeys = solutions
            .Select(s => s.AuthorProfile?.ProfilePictureObjectKey)
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

        var items = solutions.Select(s => new SolutionSummaryDto(
            Id: s.Id,
            ProblemId: s.ProblemId,
            Status: s.Status,
            Author: new ProfileSnapshotDto(
                Id: s.AuthorId,
                FullName: s.AuthorProfile?.FullName ?? string.Empty,
                Specialization: s.AuthorProfile?.Specialization,
                ProfilePictureUrl: s.AuthorProfile?.ProfilePictureObjectKey != null && urlMap.TryGetValue(s.AuthorProfile.ProfilePictureObjectKey, out var picUrl)
                    ? picUrl
                    : null),
            ContentBlocks: s.ContentBlocks.Select(cb => new SolutionContentBlockDto(
                Id: cb.Id,
                Type: cb.Type,
                Content: cb.Type == SolutionBlockType.Media && !string.IsNullOrWhiteSpace(cb.Content) && urlMap.TryGetValue(cb.Content, out var mediaUrl)
                    ? mediaUrl
                    : cb.Content,
                ExtraInfo: cb.ExtraInfo,
                Order: cb.Order
            )).ToList(),
            UpvotesCount: s.UpvotesCount,
            DownvotesCount: s.DownvotesCount,
            DiscussionsCount: s.DiscussionsCount,
            CurrentUserVote: s.CurrentUserVote,
            IsUpVotedByCurrentUser: s.IsUpVotedByCurrentUser,
            IsDownVotedByCurrentUser: s.IsDownVotedByCurrentUser,
            CreatedAt: s.CreatedAt,
            UpdatedAt: s.UpdatedAt)).ToList();

        return Result<Paged<SolutionSummaryDto>>.Success(new Paged<SolutionSummaryDto>(
            items: items,
            count: totalCount,
            pageSize: request.PageSize,
            currentPage: request.CurrentPage), OperationStatusCode.Success);
    }
}
