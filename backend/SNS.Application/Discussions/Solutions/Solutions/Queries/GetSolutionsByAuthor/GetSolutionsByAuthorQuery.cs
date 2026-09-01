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

namespace SNS.Application.Discussions.Solutions.Solutions.Queries.GetSolutionsByAuthor;

/// <summary>
/// Query to retrieve a paged list of solutions submitted by a specific author profile.
/// </summary>
/// <param name="AuthorId">The unique identifier of the author profile.</param>
/// <param name="PageSize">The maximum number of solutions per page.</param>
/// <param name="CurrentPage">The page index for pagination (1-based).</param>
public sealed record GetSolutionsByAuthorQuery(
    Guid AuthorId,
    int PageSize = 10,
    int CurrentPage = 1
) : IQuery<Paged<SolutionSummaryDto>>;

/// <summary>
/// Handles <see cref="GetSolutionsByAuthorQuery"/> to fetch solutions created by a specific user profile.
/// </summary>
internal sealed class GetSolutionsByAuthorQueryHandler : IQueryHandler<GetSolutionsByAuthorQuery, Paged<SolutionSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public GetSolutionsByAuthorQueryHandler(
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<SolutionSummaryDto>>> Handle(GetSolutionsByAuthorQuery request, CancellationToken cancellationToken)
    {
        var baseQuery = _dbContext.Solutions
            .AsNoTracking()
            .Where(s => s.AuthorId == request.AuthorId && s.IsActive);

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
