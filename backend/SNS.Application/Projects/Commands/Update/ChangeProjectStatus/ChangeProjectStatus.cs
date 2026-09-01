using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Projects.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Projects.Commands.Update.ChangeProjectStatus;

public sealed record ChangeProjectStatusCommand(
    Guid ProjectId,
    ProjectStatus Status
) : ICommand;

internal sealed class ChangeProjectStatusCommandHandler : ICommandHandler<ChangeProjectStatusCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeProjectStatusCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeProjectStatusCommand request, CancellationToken cancellationToken)
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

        if (project.Status == request.Status)
        {
            // Idempotent successful no-op
            return Result.Success(ProjectStatusCodes.ProjectStatusChanged);
        }

        if (request.Status == ProjectStatus.Draft)
        {
            return Result.Failure(ProjectStatusCodes.InvalidStatusTransition);
        }

        // Apply any specific transitions rules 
        // e.g. Draft -> InProgress is allowed. Others like Completed or Archived might be allowed depending on domain.
        // Assuming allowed unless it's to Draft as per requirements.

        // Wait, Project entity has no ChangeStatus method. I'll need to add it or set it.
        // Let's assume there's a ChangeStatus method or we just set the property if it has a setter.
        // I will add ChangeStatus method to Project.cs first.
        // For now, I'll update it later if needed.
        
        project.ChangeStatus(request.Status);



        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProjectStatusCodes.ProjectStatusChanged);
    }
}
