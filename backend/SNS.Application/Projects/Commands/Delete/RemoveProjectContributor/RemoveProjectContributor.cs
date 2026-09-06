using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;

namespace SNS.Application.Projects.Commands.Delete.RemoveProjectContributor;

public sealed record RemoveProjectContributorCommand(
    Guid ProjectId,
    Guid ContributorId
) : ICommand;

internal sealed class RemoveProjectContributorCommandHandler : ICommandHandler<RemoveProjectContributorCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly IRepository<ProjectContributor> _contributorRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveProjectContributorCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        IRepository<ProjectContributor> contributorRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _contributorRepo = contributorRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveProjectContributorCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var project = await _projectRepo.GetSingleByExpressionAsync(p => p.Id == request.ProjectId, cancellationToken);
        if (project == null)
        {
            return Result.Failure(ProjectStatusCodes.ProjectNotFound);
        }

        if (project.OwnerId != profileId.Value)
        {
            return Result.Failure(ProjectStatusCodes.NotProjectOwner);
        }

        var existingContributor = await _contributorRepo.GetSingleByExpressionAsync(
            c => c.ProjectId == request.ProjectId && c.ContributorId == request.ContributorId,
            cancellationToken);

        if (existingContributor == null)
        {
            return Result.Failure(ProjectStatusCodes.ContributorInvitationNotFound);
        }

        _contributorRepo.Delete(existingContributor);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}
