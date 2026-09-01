using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Projects.Commands.Create.AddProjectSkill;

public sealed record AddProjectSkillCommand(
    Guid ProjectId,
    Guid SkillId
) : ICommand;

internal sealed class AddProjectSkillCommandHandler : ICommandHandler<AddProjectSkillCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly IRepository<ProjectSkill> _projectSkillRepo;
    private readonly ISoftDeletableRepository<Skill> _skillRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddProjectSkillCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        IRepository<ProjectSkill> projectSkillRepo,
        ISoftDeletableRepository<Skill> skillRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _projectSkillRepo = projectSkillRepo;
        _skillRepo = skillRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddProjectSkillCommand request, CancellationToken cancellationToken)
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

        var skillExists = await _skillRepo.ExistsAsync(s => s.Id == request.SkillId, cancellationToken);

        if (!skillExists)
        {
            return Result.Failure(ProjectStatusCodes.SkillNotFound);
        }

        var existingProjectSkill = await _projectSkillRepo.GetSingleByExpressionAsync(ps => ps.ProjectId == request.ProjectId && ps.SkillId == request.SkillId, cancellationToken);

        if (existingProjectSkill != null)
        {
            return Result.Success(ProjectStatusCodes.SkillAdded);
        }

        var projectSkill = ProjectSkill.Create(request.ProjectId, request.SkillId);
        _projectSkillRepo.Add(projectSkill);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProjectStatusCodes.SkillAdded);
    }
}
