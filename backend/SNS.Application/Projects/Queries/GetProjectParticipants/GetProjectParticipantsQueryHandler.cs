using Microsoft.EntityFrameworkCore;
using SNS.Application.Projects.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Domain.Projects.Enums;
using System.Linq;

namespace SNS.Application.Projects.Queries.GetProjectParticipants;

internal sealed class GetProjectParticipantsQueryHandler : IQueryHandler<GetProjectParticipantsQuery, Paged<ProjectParticipantDetailsDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetProjectParticipantsQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Paged<ProjectParticipantDetailsDto>>> Handle(GetProjectParticipantsQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;

        var query = _dbContext.ProjectContributors
            .Where(c => c.ProjectId == request.ProjectId && c.InvitingStatus == InvitingStatus.Accepted)
            .Select(c => new ProjectParticipantDetailsDto(
                c.ContributorId,
                c.Contributor.ProfilePictureObjectKey,
                c.Contributor.FullName,
                c.Contributor.Specialization,
                c.Contributor.Followers.Count(),
                c.Contributor.Followings.Count(),
                currentProfileId.HasValue && c.Contributor.Followers.Any(f => f.FollowerId == currentProfileId.Value),
                c.Role.ToString()
            ))
            .OrderBy(p => p.ProfileId); 

        var count = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var paged = new Paged<ProjectParticipantDetailsDto>(items, count, request.PageSize, request.Page);

        return Result<Paged<ProjectParticipantDetailsDto>>.Success(paged, OperationStatusCode.Success);
    }
}
