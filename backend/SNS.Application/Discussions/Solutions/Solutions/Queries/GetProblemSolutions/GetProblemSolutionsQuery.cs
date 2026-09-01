using Microsoft.EntityFrameworkCore;
using SNS.Application.Discussions.Solutions.Solutions.Contracts;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Domain.Discussions.Shared.Enums;
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

    public GetProblemSolutionsQueryHandler(
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<SolutionSummaryDto>>> Handle(GetProblemSolutionsQuery request, CancellationToken cancellationToken)
    {
        var problemExists = await _dbContext.Problems
            .AnyAsync(p => p.Id == request.ProblemId && p.IsActive, cancellationToken);

        if (!problemExists)
        {
            return Result<Paged<SolutionSummaryDto>>.Failure(ProblemStatusCodes.ProblemNotFound);
        }

        var baseQuery = _dbContext.Solutions
            .AsNoTracking()
            .Where(s => s.ProblemId == request.ProblemId && s.IsActive);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var solutions = await baseQuery
            .OrderByDescending(s => s.Status == SNS.Domain.Discussions.Solutions.Enums.SolutionStatus.BestSolution)
            .ThenByDescending(s => s.Status == SNS.Domain.Discussions.Solutions.Enums.SolutionStatus.Accepted)
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
                UpvotesCount = s.Votes.Count(v => v.Type == VoteType.Upvote),
                DownvotesCount = s.Votes.Count(v => v.Type == VoteType.Downvote),
                DiscussionsCount = s.Discussions.Count(d => d.IsActive)
            })
            .ToListAsync(cancellationToken);

        var items = solutions.Select(s => new SolutionSummaryDto(
            Id: s.Id,
            ProblemId: s.ProblemId,
            Status: s.Status,
            Author: new ProfileSnapshotDto(
                Id: s.AuthorId,
                FullName: s.AuthorProfile?.FullName ?? string.Empty,
                Specialization: s.AuthorProfile?.Specialization,
                ProfilePictureUrl: s.AuthorProfile?.ProfilePictureObjectKey != null
                    ? _fileStorageService.GetFilePublicUrl(s.AuthorProfile.ProfilePictureObjectKey)
                    : null),
            UpvotesCount: s.UpvotesCount,
            DownvotesCount: s.DownvotesCount,
            DiscussionsCount: s.DiscussionsCount,
            CreatedAt: s.CreatedAt,
            UpdatedAt: s.UpdatedAt)).ToList();

        return Result<Paged<SolutionSummaryDto>>.Success(new Paged<SolutionSummaryDto>(
            items: items,
            count: totalCount,
            pageSize: request.PageSize,
            currentPage: request.CurrentPage), OperationStatusCode.Success);
    }
}
