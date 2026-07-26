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

public sealed record GetProfileFollowersQuery(
    Guid ProfileId,
    string? SearchTerm,
    int PageSize = 10,
    int CurrentPage = 1
): IQuery<Paged<ProfileFollowDto>>;


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