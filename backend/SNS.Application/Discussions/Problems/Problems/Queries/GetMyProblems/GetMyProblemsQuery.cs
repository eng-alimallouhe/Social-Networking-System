using Microsoft.EntityFrameworkCore;
using SNS.Application.Discussions.Problems.Problems.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Problems.Problems.Queries.GetMyProblems;

/// <summary>
/// Query to retrieve a paged list of discussion problems authored by the current authenticated user.
/// </summary>
/// <param name="PageSize">The maximum number of items per page.</param>
/// <param name="CurrentPage">The current page index (1-based).</param>
/// <param name="SearchTerm">Optional search keyword.</param>
public sealed record GetMyProblemsQuery(
    int PageSize = 10,
    int CurrentPage = 1,
    string? SearchTerm = null
) : IQuery<Paged<ProblemSummaryDto>>;

/// <summary>
/// Handles <see cref="GetMyProblemsQuery"/> to fetch authored problems with vote and solution counts.
/// </summary>
internal sealed class GetMyProblemsQueryHandler : IQueryHandler<GetMyProblemsQuery, Paged<ProblemSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetMyProblemsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<ProblemSummaryDto>>> Handle(GetMyProblemsQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<Paged<ProblemSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var baseQuery = _dbContext.Problems
            .AsNoTracking()
            .Where(p => p.AuthorId == profileId.Value && p.IsActive);

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
                AuthorName = p.Author.FullName,
                AuthorProfilePictureObjectKey = p.Author.ProfilePictureObjectKey,
                UpvotesCount = p.Votes.Count(v => v.Type == VoteType.Upvote),
                SolutionsCount = p.Solutions.Count(s => s.IsActive),
                Tags = p.ProblemTags.Select(pt => pt.Tag.Name).ToList(),
                Topics = p.ProblemTopics.Select(pt => pt.Topic.Name).ToList(),
                p.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var items = problems.Select(p => new ProblemSummaryDto(
            Id: p.Id,
            Title: p.Title,
            Status: p.Status,
            Level: p.Level,
            AuthorId: p.AuthorId,
            AuthorName: p.AuthorName,
            AuthorProfilePictureUrl: p.AuthorProfilePictureObjectKey != null
                ? _fileStorageService.GetFilePublicUrl(p.AuthorProfilePictureObjectKey)
                : null,
            UpvotesCount: p.UpvotesCount,
            SolutionsCount: p.SolutionsCount,
            Tags: p.Tags,
            Topics: p.Topics,
            CreatedAt: p.CreatedAt
        )).ToList();

        return Result<Paged<ProblemSummaryDto>>.Success(new Paged<ProblemSummaryDto>(
            items: items,
            count: totalCount,
            pageSize: request.PageSize,
            currentPage: request.CurrentPage), OperationStatusCode.Success);
    }
}
