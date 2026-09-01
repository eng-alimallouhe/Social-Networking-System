using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Projects.Commands.Delete.DeleteProjectMilestone;

public sealed record DeleteProjectMilestoneCommand(
    Guid ProjectId,
    Guid MilestoneId
) : ICommand;

internal sealed class DeleteProjectMilestoneCommandHandler : ICommandHandler<DeleteProjectMilestoneCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly IRepository<ProjectMilestone> _milestoneRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProjectMilestoneCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        IRepository<ProjectMilestone> milestoneRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _milestoneRepo = milestoneRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteProjectMilestoneCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
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

        var milestone = await _milestoneRepo.GetSingleByExpressionAsync(m => m.Id == request.MilestoneId && m.ProjectId == request.ProjectId, cancellationToken);

        if (milestone == null)
        {
            return Result.Failure(ProjectStatusCodes.MilestoneNotFound);
        }

        _milestoneRepo.Delete(milestone);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProjectStatusCodes.MilestoneRemoved);
    }
}
