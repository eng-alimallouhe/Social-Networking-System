using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.Projects.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfilesForProjectInvitation;

public sealed record GetProfilesForProjectInvitationQuery(
    Guid ProjectId,
    string? Search = null
) : IQuery<List<ProfileInvitationCandidateDto>>;

internal sealed class GetProfilesForProjectInvitationQueryHandler
    : IQueryHandler<GetProfilesForProjectInvitationQuery, List<ProfileInvitationCandidateDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetProfilesForProjectInvitationQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<List<ProfileInvitationCandidateDto>>> Handle(
        GetProfilesForProjectInvitationQuery request,
        CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<List<ProfileInvitationCandidateDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var currentProfileId = profileId.Value;

        var project = await _dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project == null)
        {
            return Result<List<ProfileInvitationCandidateDto>>.Failure(ProjectStatusCodes.ProjectNotFound);
        }

        if (project.OwnerId != currentProfileId)
        {
            return Result<List<ProfileInvitationCandidateDto>>.Failure(ProjectStatusCodes.NotProjectOwner);
        }

        // Exclude current contributors and pending invitations
        var existingContributorIds = _dbContext.ProjectContributors
            .Where(c => c.ProjectId == request.ProjectId &&
                        (c.InvitingStatus == InvitingStatus.Accepted || c.InvitingStatus == InvitingStatus.Pending))
            .Select(c => c.ContributorId);

        // Exclude blocked/blocker profiles
        var blockedUserIds = _dbContext.Blocks
            .Where(b => b.BlockerId == currentProfileId)
            .Select(b => b.BlockedId);

        var blockerUserIds = _dbContext.Blocks
            .Where(b => b.BlockedId == currentProfileId)
            .Select(b => b.BlockerId);

        // Candidate eligibility rule:
        // Candidate MUST follow current user (which covers both "Mutual Follow" and "Candidate follows current user")
        var followerUserIds = _dbContext.Follows
            .Where(f => f.FollowingId == currentProfileId)
            .Select(f => f.FollowerId);

        // Profiles followed by current user (to determine Mutual Follow priority)
        var followingUserIds = _dbContext.Follows
            .Where(f => f.FollowerId == currentProfileId)
            .Select(f => f.FollowingId);

        var query = _dbContext.Profiles
            .Where(p => p.IsActive)
            .Where(p => p.Id != currentProfileId)
            .Where(p => !existingContributorIds.Contains(p.Id))
            .Where(p => !blockedUserIds.Contains(p.Id))
            .Where(p => !blockerUserIds.Contains(p.Id))
            .Where(p => followerUserIds.Contains(p.Id));

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(p => p.FullName.Contains(term) ||
                                     (p.Specialization != null && p.Specialization.Contains(term)));
        }

        var candidates = await query
            .OrderByDescending(p => followingUserIds.Contains(p.Id))
            .ThenByDescending(p => p.Followers.Count())
            .ThenBy(p => p.FullName)
            .Take(10)
            .Select(p => new
            {
                p.Id,
                p.FullName,
                p.Specialization,
                p.ProfilePictureObjectKey,
                IsMutualFollow = followingUserIds.Contains(p.Id)
            })
            .ToListAsync(cancellationToken);

        var results = candidates.Select(c => new ProfileInvitationCandidateDto(
            Id: c.Id,
            FullName: c.FullName,
            Specialization: c.Specialization,
            ProfilePictureUrl: c.ProfilePictureObjectKey != null ? _fileStorageService.GetFilePublicUrl(c.ProfilePictureObjectKey) : null,
            IsMutualFollow: c.IsMutualFollow,
            FollowsCurrentUser: true
        )).ToList();

        return Result<List<ProfileInvitationCandidateDto>>.Success(results, ResourceStatusCode.Found);
    }
}
