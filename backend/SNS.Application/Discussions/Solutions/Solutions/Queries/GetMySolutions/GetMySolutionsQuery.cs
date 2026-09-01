using Microsoft.EntityFrameworkCore;
using SNS.Application.Discussions.Solutions.Solutions.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Solutions.Solutions.Queries.GetMySolutions;

/// <summary>
/// Query to retrieve a paged list of solutions submitted by the authenticated user.
/// </summary>
/// <param name="PageSize">The maximum number of solutions to return per page.</param>
/// <param name="CurrentPage">The page index for pagination (1-based).</param>
public sealed record GetMySolutionsQuery(
    int PageSize = 10,
    int CurrentPage = 1
) : IQuery<Paged<SolutionSummaryDto>>;

/// <summary>
/// Handles <see cref="GetMySolutionsQuery"/> to fetch user's solutions.
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
