using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Projects.Commands.Create.AddProjectMilestone;

public sealed record AddProjectMilestoneCommand(
    Guid ProjectId,
    string Title,
    string Description
) : ICommand;

internal sealed class AddProjectMilestoneCommandHandler : ICommandHandler<AddProjectMilestoneCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly IRepository<ProjectMilestone> _milestoneRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddProjectMilestoneCommandHandler(
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

    public async Task<Result> Handle(AddProjectMilestoneCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
        {
            return Result.Failure(ProjectStatusCodes.InvalidStatusTransition); // Generic validation error code
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

        var milestone = ProjectMilestone.Create(request.ProjectId, request.Title, request.Description);
        
        _milestoneRepo.Add(milestone);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProjectStatusCodes.MilestoneAdded);
    }
}
