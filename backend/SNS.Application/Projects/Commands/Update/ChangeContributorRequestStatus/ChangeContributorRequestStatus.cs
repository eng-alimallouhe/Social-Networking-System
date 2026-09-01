using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Projects.Enums;
using SNS.Domain.Projects.Events;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Projects.Commands.Update.ChangeContributorRequestStatus;

public sealed record ChangeContributorRequestStatusCommand(
    Guid ProjectId,
    bool IsAccepted
) : ICommand;

internal sealed class ChangeContributorRequestStatusCommandHandler : ICommandHandler<ChangeContributorRequestStatusCommand>
{
    private readonly IRepository<ProjectContributor> _contributorRepo;
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeContributorRequestStatusCommandHandler(
        IRepository<ProjectContributor> contributorRepo,
        ISoftDeletableRepository<Project> projectRepo,
        ISoftDeletableRepository<Profile> profileRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _contributorRepo = contributorRepo;
        _projectRepo = projectRepo;
        _profileRepo = profileRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeContributorRequestStatusCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var contributor = await _contributorRepo.GetSingleByExpressionAsync(c => c.ProjectId == request.ProjectId && c.ContributorId == profileId.Value, cancellationToken);

        if (contributor == null)
        {
            return Result.Failure(ProjectStatusCodes.ContributorInvitationNotFound);
        }

        if (contributor.InvitingStatus != InvitingStatus.Pending)
        {
            return Result.Failure(ProjectStatusCodes.InvalidInvitationStatusTransition);
        }

        var project = await _projectRepo.GetSingleByExpressionAsync(p => p.Id == request.ProjectId, cancellationToken);
            
        var invitedProfile = await _profileRepo.GetSingleByExpressionAsync(p => p.Id == profileId.Value, cancellationToken);

        if (request.IsAccepted)
        {
            contributor.AcceptInvitation();
        }
        else
        {
            contributor.RejectInvitation();
        }

        if (project != null && invitedProfile != null)
        {
            contributor.AddDomainEvent(new ProjectContributorInvitationRespondedEvent(
                ProjectId: request.ProjectId,
                ProjectOwnerProfileId: project.OwnerId,
                InvitedUserName: invitedProfile.FullName,
                IsAccepted: request.IsAccepted,
                OccurredOn: DateTime.UtcNow
            ));
        }



        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProjectStatusCodes.InvitationStatusUpdated);
    }
}
