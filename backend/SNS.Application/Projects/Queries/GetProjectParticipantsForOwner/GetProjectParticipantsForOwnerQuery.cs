using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Projects.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Domain.Projects.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;

namespace SNS.Application.Projects.Queries.GetProjectParticipantsForOwner;

public sealed record GetProjectParticipantsForOwnerQuery(
    Guid ProjectId,
    int Page = 1,
    int PageSize = 20
) : IQuery<Paged<ProjectContributorManagementDto>>;

internal sealed class GetProjectParticipantsForOwnerQueryHandler
    : IQueryHandler<GetProjectParticipantsForOwnerQuery, Paged<ProjectContributorManagementDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetProjectParticipantsForOwnerQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<ProjectContributorManagementDto>>> Handle(
        GetProjectParticipantsForOwnerQuery request,
        CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<Paged<ProjectContributorManagementDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var project = await _dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project == null)
        {
            return Result<Paged<ProjectContributorManagementDto>>.Failure(ProjectStatusCodes.ProjectNotFound);
        }

        if (project.OwnerId != profileId.Value)
        {
            return Result<Paged<ProjectContributorManagementDto>>.Failure(ProjectStatusCodes.NotProjectOwner);
        }

        var baseQuery = _dbContext.ProjectContributors
            .Where(c => c.ProjectId == request.ProjectId &&
                        (c.InvitingStatus == InvitingStatus.Accepted || c.InvitingStatus == InvitingStatus.Pending));

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var contributors = await baseQuery
            .OrderBy(c => c.InvitingStatus == InvitingStatus.Pending ? 0 : 1)
            .ThenByDescending(c => c.InvitationSentAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new
            {
                c.Id,
                c.ContributorId,
                c.Contributor.ProfilePictureObjectKey,
                DisplayName = c.Contributor.FullName,
                c.Contributor.Specialization,
                FollowersCount = c.Contributor.Followers.Count(),
                FollowingCount = c.Contributor.Followings.Count(),
                IsFollowedByCurrentUser = c.Contributor.Followers.Any(f => f.FollowerId == profileId.Value),
                Role = c.Role.ToString(),
                c.InvitingStatus,
                c.InvitationSentAt,
                c.RespondedAt,
                c.InvitationMessage
            })
            .ToListAsync(cancellationToken);

        var items = contributors.Select(c => new ProjectContributorManagementDto(
            ContributorRecordId: c.Id,
            ProfileId: c.ContributorId,
            ProfileImageUrl: c.ProfilePictureObjectKey != null ? _fileStorageService.GetFilePublicUrl(c.ProfilePictureObjectKey) : null,
            DisplayName: c.DisplayName,
            Specialization: c.Specialization,
            FollowersCount: c.FollowersCount,
            FollowingCount: c.FollowingCount,
            IsFollowedByCurrentUser: c.IsFollowedByCurrentUser,
            Role: c.Role,
            InvitingStatus: c.InvitingStatus,
            InvitationSentAt: c.InvitationSentAt,
            RespondedAt: c.RespondedAt,
            InvitationMessage: c.InvitationMessage
        )).ToList();

        var paged = new Paged<ProjectContributorManagementDto>(items, totalCount, request.PageSize, request.Page);

        return Result<Paged<ProjectContributorManagementDto>>.Success(paged, OperationStatusCode.Success);
    }
}
