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
using SNS.Shared.StatusCodes.Discussions;

namespace SNS.Application.Discussions.Solutions.Solutions.Queries.GetProblemSolutions;

/// <summary>
/// Query to retrieve a paged list of solutions proposed for a specific discussion problem.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem.</param>
/// <param name="PageSize">The maximum number of solutions per page.</param>
/// <param name="CurrentPage">The page index for pagination (1-based).</param>
public sealed record GetProblemSolutionsQuery(
    Guid ProblemId,
    int PageSize = 10,
    int CurrentPage = 1
) : IQuery<Paged<SolutionSummaryDto>>;

/// <summary>
/// Handles <see cref="GetProblemSolutionsQuery"/> to fetch solutions for a given problem.
/// </summary>
internal sealed class GetProblemSolutionsQueryHandler : IQueryHandler<GetProblemSolutionsQuery, Paged<SolutionSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;

    public GetProblemSolutionsQueryHandler(
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Paged<SolutionSummaryDto>>> Handle(GetProblemSolutionsQuery request, CancellationToken cancellationToken)
    {
        var problemExists = await _dbContext.Problems
            .AnyAsync(p => p.Id == request.ProblemId && p.IsActive, cancellationToken);

        if (!problemExists)
        {
            return Result<Paged<SolutionSummaryDto>>.Failure(ProblemStatusCodes.ProblemNotFound);
        }

        var currentProfileId = _currentUserService.ProfileId;

        var baseQuery = _dbContext.Solutions
            .AsNoTracking()
            .Where(s => s.ProblemId == request.ProblemId && s.IsActive);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var solutions = await baseQuery
            .OrderByDescending(s => s.Status == SolutionStatus.BestSolution)
            .ThenByDescending(s => s.Status == SolutionStatus.Accepted)
            .ThenByDescending(s => s.CreatedAt)
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
                CurrentUserVote = currentProfileId.HasValue
                    ? s.Votes.Where(v => v.VoterId == currentProfileId.Value).Select(v => (VoteType?)v.Type).FirstOrDefault()
                    : null,
                IsUpVotedByCurrentUser = currentProfileId.HasValue && s.Votes.Any(v => v.VoterId == currentProfileId.Value && v.Type == VoteType.Upvote),
                IsDownVotedByCurrentUser = currentProfileId.HasValue && s.Votes.Any(v => v.VoterId == currentProfileId.Value && v.Type == VoteType.Downvote)
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
