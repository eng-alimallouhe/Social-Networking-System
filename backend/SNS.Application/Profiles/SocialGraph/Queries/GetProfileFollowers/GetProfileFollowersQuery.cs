using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.SocialGraph.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Profiles.SocialGraph.Queries.GetProfileFollowers;

/// <summary>
/// Represents a query to retrieve a paged list of followers for a specified profile.
/// </summary>
/// <param name="ProfileId">The unique identifier of the profile whose followers are being queried.</param>
/// <param name="SearchTerm">Optional search term to filter followers by full name or specialization.</param>
/// <param name="PageSize">The maximum number of follower records to return per page.</param>
/// <param name="CurrentPage">The page index for pagination (1-based).</param>
public sealed record GetProfileFollowersQuery(
    Guid ProfileId,
    string? SearchTerm,
    int PageSize = 10,
    int CurrentPage = 1
): IQuery<Paged<ProfileFollowDto>>;

/// <summary>
/// Handles the execution of <see cref="GetProfileFollowersQuery"/> to retrieve profile followers.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Queries follow records where the specified profile is being followed.
/// 2. Applies search term filtering on full name and specialization if specified.
/// 3. Sorts by follow date descending and applies pagination (<c>Skip</c>/<c>Take</c>).
/// 4. Maps profile picture keys to public storage URLs and packages results in <see cref="Paged{ProfileFollowDto}"/>.
/// </remarks>
internal sealed class GetProfileFollowersQueryHandler
    : IQueryHandler<GetProfileFollowersQuery, Paged<ProfileFollowDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public GetProfileFollowersQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<ProfileFollowDto>>> Handle(GetProfileFollowersQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext
            .Follows
            .Where(pv => pv.FollowingId == request.ProfileId);

        if (request.SearchTerm != null)
        {
            var search = request.SearchTerm.Trim();

            query = query.Where(f =>
                f.Follower.FullName.Contains(search) ||
                (f.Follower.Specialization != null &&
                 f.Follower.Specialization.Contains(search)));
        }

        var profileViewsCount = await query.CountAsync(cancellationToken);

        var followers = await query
            .OrderByDescending(pv => pv.CreatedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(pv => new
            {
                ProfileId = pv.FollowerId,
                FullName = pv.Follower.FullName,
                ProfilePictureObjectKey = pv.Follower.ProfilePictureObjectKey,
                Specialization = pv.Follower.Specialization,
                FollowDate = pv.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var followersResult = followers.Select(pv => new ProfileFollowDto
        (
            ProfileId: pv.ProfileId,
            FullName: pv.FullName,
            ProfilePictureUrl: pv.ProfilePictureObjectKey != null ? _fileStorageService.GetFilePublicUrl(pv.ProfilePictureObjectKey) : null,
            Specialization: pv.Specialization,
            FollowDate: pv.FollowDate
        )).ToList();

        return Result<Paged<ProfileFollowDto>>.Success(new Paged<ProfileFollowDto>(
            items: followersResult,
            currentPage: request.CurrentPage,
            pageSize: request.PageSize,
            count: profileViewsCount),
            ResourceStatusCode.Found);
    }
}