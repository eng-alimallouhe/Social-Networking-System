using Microsoft.EntityFrameworkCore;
using SNS.Application.Discussions.Problems.ProblemViews.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Problems.ProblemViews.Queries.GetProblemViewers;

/// <summary>
/// Query to retrieve a paged list of viewer profiles for a discussion problem.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem.</param>
/// <param name="PageSize">The maximum number of viewer records per page.</param>
/// <param name="CurrentPage">The page index for pagination (1-based).</param>
/// <param name="SearchTerm">Optional keyword to filter viewers by name or specialization.</param>
public sealed record GetProblemViewersQuery(
    Guid ProblemId,
    int PageSize = 10,
    int CurrentPage = 1,
    string? SearchTerm = null
) : IQuery<Paged<ProblemViewerDto>>;

/// <summary>
/// Handles <see cref="GetProblemViewersQuery"/> to fetch viewers of a problem.
/// </summary>
internal sealed class GetProblemViewersQueryHandler : IQueryHandler<GetProblemViewersQuery, Paged<ProblemViewerDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetProblemViewersQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<ProblemViewerDto>>> Handle(GetProblemViewersQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<Paged<ProblemViewerDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var problem = await _dbContext.Problems
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.ProblemId && p.IsActive, cancellationToken);

        if (problem == null)
        {
            return Result<Paged<ProblemViewerDto>>.Failure(ProblemStatusCodes.ProblemNotFound);
        }

        if (problem.AuthorId != profileId.Value)
        {
            return Result<Paged<ProblemViewerDto>>.Failure(ProblemStatusCodes.NotProblemOwner);
        }

        var baseQuery = _dbContext.ProblemViews
            .AsNoTracking()
            .Where(v => v.ProblemId == request.ProblemId && v.IsActive);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim().ToLower();
            var matchedViewerIds = _dbContext.Profiles
                .Where(p => p.FullName.ToLower().Contains(search) || (p.Specialization != null && p.Specialization.ToLower().Contains(search)))
                .Select(p => p.Id);

            baseQuery = baseQuery.Where(v => matchedViewerIds.Contains(v.ViewerId));
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var views = await baseQuery
            .OrderByDescending(v => v.ViewedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(v => new
            {
                v.ViewerId,
                v.ViewedAt
            })
            .ToListAsync(cancellationToken);

        var viewerIds = views.Select(v => v.ViewerId).Distinct().ToList();

        var profiles = await _dbContext.Profiles
            .AsNoTracking()
            .Where(p => viewerIds.Contains(p.Id))
            .Select(p => new
            {
                p.Id,
                p.FullName,
                p.Specialization,
                p.ProfilePictureObjectKey
            })
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        var items = views.Select(v =>
        {
            profiles.TryGetValue(v.ViewerId, out var prof);
            return new ProblemViewerDto(
                ProfileId: v.ViewerId,
                FullName: prof?.FullName ?? string.Empty,
                Specialization: prof?.Specialization,
                ProfilePictureUrl: prof?.ProfilePictureObjectKey != null
                    ? _fileStorageService.GetFilePublicUrl(prof.ProfilePictureObjectKey)
                    : null,
                ViewedAt: v.ViewedAt);
        }).ToList();

        return Result<Paged<ProblemViewerDto>>.Success(new Paged<ProblemViewerDto>(
            items: items,
            count: totalCount,
            pageSize: request.PageSize,
            currentPage: request.CurrentPage), OperationStatusCode.Success);
    }
}
