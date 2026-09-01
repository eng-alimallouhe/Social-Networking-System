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

namespace SNS.Application.Projects.Commands.Delete.RemoveProjectSkill;

public sealed record RemoveProjectSkillCommand(
    Guid ProjectId,
    Guid ProjectSkillId
) : ICommand;

internal sealed class RemoveProjectSkillCommandHandler : ICommandHandler<RemoveProjectSkillCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly IRepository<ProjectSkill> _projectSkillRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveProjectSkillCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        IRepository<ProjectSkill> projectSkillRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _projectSkillRepo = projectSkillRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveProjectSkillCommand request, CancellationToken cancellationToken)
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

        var existingProjectSkill = await _projectSkillRepo.GetSingleByExpressionAsync(ps => ps.ProjectId == request.ProjectId && ps.Id == request.ProjectSkillId, cancellationToken);

        if (existingProjectSkill != null)
        {
            _projectSkillRepo.Delete(existingProjectSkill);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        return Result.Success(ProjectStatusCodes.SkillRemoved);
    }
}
