using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Projects.Commands.Create.AddProjectMedia;

public sealed record AddProjectMediaCommand(
    Guid ProjectId,
    Stream FileStream,
    string ContentType,
    string FileName,
    string Caption,
    MediaType Type
) : ICommand;

internal sealed class AddProjectMediaCommandHandler : ICommandHandler<AddProjectMediaCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly IRepository<ProjectMedia> _projectMediaRepo;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddProjectMediaCommandHandler(
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

    public async Task<Result> Handle(AddProjectMediaCommand request, CancellationToken cancellationToken)
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

        var objectKey = $"projects/{request.ProjectId}/media/{Guid.NewGuid()}_{request.FileName}";
        
        var mediaUrl = await _fileStorageService.UploadFileAsync(
            request.FileStream,
            request.ContentType,
            objectKey,
            cancellationToken);

        var media = ProjectMedia.Create(request.ProjectId, mediaUrl, request.Caption, request.Type);
        
        _projectMediaRepo.Add(media);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProjectStatusCodes.MediaAdded);
    }
}
