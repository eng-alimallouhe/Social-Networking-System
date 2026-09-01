using Microsoft.EntityFrameworkCore;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Resumes.Resumes.Contracts;
using SNS.Application.Resumes.Resumes.Services;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Resumes.Resumes.Queries.GetMyResumes;

/// <summary>
/// Represents a query to retrieve all active resumes owned by the authenticated user.
/// </summary>
public sealed record GetMyResumesQuery : IQuery<List<ResumeSummaryDto>>;

/// <summary>
/// Handles the execution of <see cref="GetMyResumesQuery"/> to retrieve summary cards for the user's resumes.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Resolves authenticated user profile ID.
/// 2. Queries all active resume aggregate roots owned by the profile.
/// 3. Resolves personal and profile picture presigned temporary URLs in a single batch pass.
/// 4. Maps results to <see cref="ResumeSummaryDto"/> list.
/// </remarks>
internal sealed class GetMyResumesQueryHandler : IQueryHandler<GetMyResumesQuery, List<ResumeSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IResumeUrlResolver _resumeUrlResolver;

    public GetMyResumesQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IResumeUrlResolver resumeUrlResolver)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _resumeUrlResolver = resumeUrlResolver;
    }

    public async Task<Result<List<ResumeSummaryDto>>> Handle(GetMyResumesQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result<List<ResumeSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var resumes = await _dbContext.Resumes
            .Where(r => r.OwnerId == profileId.Value && r.IsActive)
            .OrderByDescending(r => r.UpdatedAt)
            .Select(r => new
            {
                r.Id,
                r.OwnerId,
                r.PersonalPictureUrl,
                r.SyncProfilePicture,
                r.Title,
                r.Template,
                r.Summary,
                r.Langauge,
                r.CreatedAt,
                r.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        if (!resumes.Any())
        {
            return Result<List<ResumeSummaryDto>>.Success(new List<ResumeSummaryDto>(), ResourceStatusCode.Found);
        }

        var resolutionRequests = resumes.Select(r => new ResumePictureResolutionRequest(
            r.Id,
            r.OwnerId,
            r.PersonalPictureUrl,
            r.SyncProfilePicture
        ));

        var urlMap = await _resumeUrlResolver.ResolvePersonalPictureUrlsBatchAsync(resolutionRequests, cancellationToken);

        var summaries = resumes.Select(r => new ResumeSummaryDto(
            r.Id,
            r.OwnerId,
            urlMap.TryGetValue(r.Id, out var url) ? url : null,
            r.SyncProfilePicture,
            r.Title,
            r.Template,
            r.Summary,
            r.Langauge,
            r.CreatedAt,
            r.UpdatedAt
        )).ToList();

        return Result<List<ResumeSummaryDto>>.Success(summaries, ResourceStatusCode.Found);
    }
}
