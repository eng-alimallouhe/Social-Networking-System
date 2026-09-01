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

namespace SNS.Application.Projects.Commands.Create.AddProjectTag;

public sealed record AddProjectTagCommand(
    Guid ProjectId,
    Guid TagId
) : ICommand;

internal sealed class AddProjectTagCommandHandler : ICommandHandler<AddProjectTagCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly IRepository<ProjectTag> _projectTagRepo;
    private readonly IRepository<Tag> _tagRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddProjectTagCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        IRepository<ProjectTag> projectTagRepo,
        IRepository<Tag> tagRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _projectTagRepo = projectTagRepo;
        _tagRepo = tagRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddProjectTagCommand request, CancellationToken cancellationToken)
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

        var tagExists = await _tagRepo.ExistsAsync(t => t.Id == request.TagId, cancellationToken);

        if (!tagExists)
        {
            return Result.Failure(ProjectStatusCodes.TagNotFound);
        }

        var existingProjectTag = await _projectTagRepo.GetSingleByExpressionAsync(pt => pt.ProjectId == request.ProjectId && pt.TagId == request.TagId, cancellationToken);

        if (existingProjectTag != null)
        {
            return Result.Success(ProjectStatusCodes.TagAdded);
        }

        var projectTag = ProjectTag.Create(request.ProjectId, request.TagId);
        _projectTagRepo.Add(projectTag);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProjectStatusCodes.TagAdded);
    }
}
