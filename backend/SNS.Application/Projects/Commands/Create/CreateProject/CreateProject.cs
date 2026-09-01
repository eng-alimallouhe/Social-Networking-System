using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Projects.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Projects.Commands.Create.CreateProject;

public sealed record CreateProjectCommand(
    string Title,
    string ShortDescription,
    string GitHubUrl,
    string LiveDemoUrl,
    ProjectType Type,
    List<Guid> SkillIds,
    List<Guid> TagIds
) : ICommand<Guid>;

internal sealed class CreateProjectCommandHandler : ICommandHandler<CreateProjectCommand, Guid>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly ISoftDeletableRepository<Skill> _skillRepo;
    private readonly IRepository<Tag> _tagRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        ISoftDeletableRepository<Skill> skillRepo,
        IRepository<Tag> tagRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _skillRepo = skillRepo;
        _tagRepo = tagRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.ShortDescription))
        {
            return Result<Guid>.Failure(ProjectStatusCodes.InvalidStatusTransition); // Replace with appropriate validation error code if exists, else return failure
        }
        
        //if (request.SkillIds == null || !request.SkillIds.Any())
        //{
        //    return Result.Failure(ProjectStatusCodes.SkillNotFound);
        //}

        //if (request.TagIds == null || !request.TagIds.Any())
        //{
        //    return Result.Failure(ProjectStatusCodes.TagNotFound);
        //}

        foreach (var skillId in request.SkillIds)
        {
            if (!await _skillRepo.ExistsAsync(s => s.Id == skillId, cancellationToken))
            {
                return Result<Guid>.Failure(ProjectStatusCodes.SkillNotFound);
            }
        }

        foreach (var tagId in request.TagIds)
        {
            if (!await _tagRepo.ExistsAsync(t => t.Id == tagId, cancellationToken))
            {
                return Result<Guid>.Failure(ProjectStatusCodes.TagNotFound);
            }
        }

        var project = Project.Create(
            ownerId: profileId.Value,
            title: request.Title,
            shortDescription: request.ShortDescription,
            mainImageUrl: string.Empty,
            readmeContent: $"# {request.Title}",
            gitHubUrl: request.GitHubUrl ?? string.Empty,
            liveDemoUrl: request.LiveDemoUrl ?? string.Empty,
            type: request.Type,
            status: ProjectStatus.Draft
        );

        foreach (var skillId in request.SkillIds)
        {
            project.Skills.Add(ProjectSkill.Create(project.Id, skillId));
        }

        foreach (var tagId in request.TagIds)
        {
            project.Tags.Add(ProjectTag.Create(project.Id, tagId));
        }

        _projectRepo.Add(project);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(project.Id, ProjectStatusCodes.ProjectCreated);
    }
}
