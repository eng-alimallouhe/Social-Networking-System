using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Projects.Commands.Interaction.UnsaveProject;

public sealed record UnsaveProjectCommand(
    Guid ProjectId
) : ICommand;

internal sealed class UnsaveProjectCommandHandler : ICommandHandler<UnsaveProjectCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly IRepository<SavedProject> _savedProjectRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UnsaveProjectCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        IRepository<SavedProject> savedProjectRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _savedProjectRepo = savedProjectRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UnsaveProjectCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var projectExists = await _projectRepo.ExistsAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (!projectExists)
        {
            return Result.Failure(ProjectStatusCodes.ProjectNotFound);
        }

        var savedProject = await _savedProjectRepo.GetSingleByExpressionAsync(s => s.ProjectId == request.ProjectId && s.ProfileId == profileId.Value, cancellationToken);

        if (savedProject != null)
        {
            _savedProjectRepo.Delete(savedProject);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        return Result.Success(ProjectStatusCodes.ProjectUnsaved);
    }
}
