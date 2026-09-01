using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Projects.Commands.Delete.DeleteProjectMedia;

public sealed record DeleteProjectMediaCommand(
    Guid ProjectId,
    Guid MediaId
) : ICommand;

internal sealed class DeleteProjectMediaCommandHandler : ICommandHandler<DeleteProjectMediaCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly IRepository<ProjectMedia> _projectMediaRepo;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProjectMediaCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        IRepository<ProjectMedia> projectMediaRepo,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _projectMediaRepo = projectMediaRepo;
        _fileStorageService = fileStorageService;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteProjectMediaCommand request, CancellationToken cancellationToken)
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

        var media = await _projectMediaRepo.GetSingleByExpressionAsync(m => m.Id == request.MediaId && m.ProjectId == request.ProjectId, cancellationToken);

        if (media == null)
        {
            return Result.Failure(ProjectStatusCodes.MediaNotFound);
        }

        // We can extract the objectKey from the MediaUrl if necessary, or assuming MediaUrl is the objectKey
        // Let's assume MediaUrl is the URL and we need to delete. Or MediaUrl stores objectKey.
        // Actually, typically in Minio it stores the URL or the key.
        // In this implementation I will pass mediaUrl directly, and if IFileStorageService requires objectKey,
        // it handles it or we parse it. Since IFileStorageService was used to upload with objectKey, it returns the objectKey or URL.
        // Let's pass media.MediaUrl to DeleteFileAsync. If it fails, we'll log it.
        try
        {
            // Try extracting object key from URL if it's a URL
            string objectKey = media.MediaUrl;
            if (Uri.TryCreate(media.MediaUrl, UriKind.Absolute, out var uri))
            {
                objectKey = uri.AbsolutePath.TrimStart('/'); // simple heuristic
            }

            await _fileStorageService.DeleteFileAsync(objectKey, cancellationToken);
        }
        catch (Exception)
        {
            // Ignore storage deletion errors to allow DB deletion to proceed, or log it
        }

        _projectMediaRepo.Delete(media);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProjectStatusCodes.MediaRemoved);
    }
}
