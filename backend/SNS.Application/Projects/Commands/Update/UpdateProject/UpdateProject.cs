using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Projects.Commands.Update.UpdateProject;

public sealed record UpdateProjectCommand(
    Guid ProjectId,
    string Title,
    string ShortDescription,
    string LiveDemoUrl
) : ICommand<Guid>;

internal sealed class UpdateProjectCommandHandler : ICommandHandler<UpdateProjectCommand, Guid>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.ShortDescription))
        {
            return Result<Guid>.Failure(ProjectStatusCodes.InvalidStatusTransition); // Generic validation error
        }

        var project = await _projectRepo.GetSingleByExpressionAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project == null)
        {
            return Result<Guid>.Failure(ProjectStatusCodes.ProjectNotFound);
        }

        if (project.OwnerId != profileId.Value)
        {
            return Result<Guid>.Failure(ProjectStatusCodes.NotProjectOwner);
        }

        project.UpdateInfo(request.Title, request.ShortDescription, request.LiveDemoUrl ?? string.Empty);



        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(project.Id, ProjectStatusCodes.ProjectUpdated);
    }
}
