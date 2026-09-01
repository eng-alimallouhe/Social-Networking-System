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

namespace SNS.Application.Projects.Commands.Delete.RemoveProjectTag;

public sealed record RemoveProjectTagCommand(
    Guid ProjectId,
    Guid ProjectTagId
) : ICommand;

internal sealed class RemoveProjectTagCommandHandler : ICommandHandler<RemoveProjectTagCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly IRepository<ProjectTag> _projectTagRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveProjectTagCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        IRepository<ProjectTag> projectTagRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _projectTagRepo = projectTagRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveProjectTagCommand request, CancellationToken cancellationToken)
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

        var existingProjectTag = await _projectTagRepo.GetSingleByExpressionAsync(pt => pt.ProjectId == request.ProjectId && pt.Id == request.ProjectTagId, cancellationToken);

        if (existingProjectTag != null)
        {
            _projectTagRepo.Delete(existingProjectTag);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        return Result.Success(ProjectStatusCodes.TagRemoved);
    }
}
