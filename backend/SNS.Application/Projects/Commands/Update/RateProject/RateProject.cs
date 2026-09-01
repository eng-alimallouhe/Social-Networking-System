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

namespace SNS.Application.Projects.Commands.Update.RateProject;

public sealed record RateProjectCommand(
    Guid ProjectId,
    int RatingValue,
    string Comment
) : ICommand;

internal sealed class RateProjectCommandHandler : ICommandHandler<RateProjectCommand>
{
    private readonly ISoftDeletableRepository<Project> _projectRepo;
    private readonly IRepository<ProjectRating> _ratingRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RateProjectCommandHandler(
        ISoftDeletableRepository<Project> projectRepo,
        IRepository<ProjectRating> ratingRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepo = projectRepo;
        _ratingRepo = ratingRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RateProjectCommand request, CancellationToken cancellationToken)
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

        var existingRating = await _ratingRepo.GetSingleByExpressionAsync(r => r.ProjectId == request.ProjectId && r.RaterId == profileId.Value, cancellationToken);

        if (existingRating != null)
        {
            existingRating.Update(request.RatingValue, request.Comment ?? string.Empty);

        }
        else
        {
            var newRating = ProjectRating.Create(profileId.Value, request.ProjectId, request.RatingValue, request.Comment ?? string.Empty);
            _ratingRepo.Add(newRating);
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProjectStatusCodes.RatingSubmitted);
    }
}
