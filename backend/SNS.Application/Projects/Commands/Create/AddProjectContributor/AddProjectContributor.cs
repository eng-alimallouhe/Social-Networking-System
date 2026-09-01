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

namespace SNS.Application.Projects.Commands.Create.AddProjectContributor;

public sealed record AddProjectContributorCommand(
    Guid ProjectId,
    Guid TargetProfileId,
    ProjectRole Role,
    string InvitationMessage
) : ICommand;

internal sealed class AddProjectContributorCommandHandler : ICommandHandler<AddProjectContributorCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly IRepository<ProjectContributor> _contributorRepo;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddProjectContributorCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        IRepository<ProjectContributor> contributorRepo,
        ISoftDeletableRepository<Profile> profileRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _contributorRepo = contributorRepo;
        _profileRepo = profileRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddProjectContributorCommand request, CancellationToken cancellationToken)
    {
        var ownerProfileId = _currentUserService.ProfileId;
        if (ownerProfileId == null)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var project = await _projectRepo.GetSingleByExpressionAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project == null)
        {
            return Result.Failure(ProjectStatusCodes.ProjectNotFound);
        }

        if (project.OwnerId != ownerProfileId.Value)
        {
            return Result.Failure(ProjectStatusCodes.NotProjectOwner);
        }

        var invitedProfile = await _profileRepo.GetSingleByExpressionAsync(p => p.Id == request.TargetProfileId, cancellationToken);

        if (invitedProfile == null)
        {
            return Result.Failure(ProjectStatusCodes.ProjectNotFound); // Using existing status or profile not found
        }

        var ownerProfile = await _profileRepo.GetSingleByExpressionAsync(p => p.Id == ownerProfileId.Value, cancellationToken);

        var existingContributor = await _contributorRepo.GetSingleByExpressionAsync(c => c.ProjectId == request.ProjectId && c.ContributorId == request.TargetProfileId, cancellationToken);

        if (existingContributor != null)
        {
            return Result.Success(ProjectStatusCodes.ContributorInvited); // No-op if already invited
        }

        var contributor = ProjectContributor.Create(request.ProjectId, request.TargetProfileId, request.Role, request.InvitationMessage ?? string.Empty);
        
        contributor.AddDomainEvent(new ProjectContributorInvitationSentEvent(
            ProjectName: project.Title,
            ProjectOwnerName: ownerProfile?.FullName ?? string.Empty,
            ProjectOwnerProfileImageUrl: ownerProfile?.ProfilePictureObjectKey ?? string.Empty,
            InvitedProfileId: request.TargetProfileId,
            OccurredOn: DateTime.UtcNow
        ));

        _contributorRepo.Add(contributor);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProjectStatusCodes.ContributorInvited);
    }
}
