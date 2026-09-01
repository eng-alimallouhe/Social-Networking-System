using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Projects;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Projects.Commands.Interaction.RecordProjectView;

public sealed record RecordProjectViewCommand(
    Guid ProjectId
) : ICommand;

internal sealed class RecordProjectViewCommandHandler : ICommandHandler<RecordProjectViewCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly ISoftDeletableRepository<ProjectView> _projectViewRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    private readonly IRequestInfoService _requestInfoService;

    public RecordProjectViewCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        ISoftDeletableRepository<ProjectView> projectViewRepo,
        ICurrentUserService currentUserService,
        IRequestInfoService requestInfoService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _projectViewRepo = projectViewRepo;
        _currentUserService = currentUserService;
        _requestInfoService = requestInfoService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RecordProjectViewCommand request, CancellationToken cancellationToken)
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

        var hasActiveView = await _projectViewRepo.ExistsAsync(pv => pv.ProjectId == request.ProjectId && pv.ViewerId == profileId.Value && pv.IsActive, cancellationToken);

        if (hasActiveView)
        {
            return Result.Success(ProjectStatusCodes.ViewRecorded); // No-op if active view already exists
        }

        var os = _requestInfoService.OperatingSystem?.ToLowerInvariant();
        DeviceType? deviceType = os switch
        {
            not null when os.Contains("windows") || os.Contains("mac") || os.Contains("linux") => DeviceType.Desktop,
            not null when os.Contains("android") || os.Contains("ios") => DeviceType.Mobile,
            _ => null
        };
        
        var ipHash = _requestInfoService.IpAddress;
        var country = _requestInfoService.Country;

        var projectView = ProjectView.Create(request.ProjectId, profileId.Value, deviceType, ipHash, country);
        _projectViewRepo.Add(projectView);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProjectStatusCodes.ViewRecorded);
    }
}
