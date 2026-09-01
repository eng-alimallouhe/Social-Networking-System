using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;

namespace SNS.Application.Projects.Commands.Update.UpdateProjectReadme;

public sealed record UpdateProjectReadmeCommand(
    Guid ProjectId,
    string ReadmeContent
) : ICommand;

internal sealed class UpdateProjectReadmeCommandHandler : ICommandHandler<UpdateProjectReadmeCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectReadmeCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateProjectReadmeCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (string.IsNullOrWhiteSpace(request.ReadmeContent))
        {
            return Result.Failure(ProjectStatusCodes.InvalidStatusTransition);
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

        project.UpdateReadmeContent(request.ReadmeContent);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProjectStatusCodes.ReadmeUpdated);
    }
}
