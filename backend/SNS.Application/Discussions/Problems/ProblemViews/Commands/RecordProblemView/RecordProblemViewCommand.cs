using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Problems.ProblemViews.Commands.RecordProblemView;

/// <summary>
/// Command to record a view interaction on a discussion problem.
/// </summary>
/// <param name="ProblemId">The unique identifier of the viewed problem.</param>
/// <param name="DeviceType">Optional device classification of the viewer.</param>
/// <param name="IpHash">Optional hashed IP address for analytics.</param>
/// <param name="Country">Optional geographic country of origin.</param>
public sealed record RecordProblemViewCommand(
    Guid ProblemId,
    DeviceType? DeviceType = null,
    string? IpHash = null,
    string? Country = null
) : ICommand;

/// <summary>
/// Handles <see cref="RecordProblemViewCommand"/> to record problem views.
/// </summary>
internal sealed class RecordProblemViewCommandHandler : ICommandHandler<RecordProblemViewCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<ProblemView> _viewRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RecordProblemViewCommandHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<ProblemView> viewRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _viewRepo = viewRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RecordProblemViewCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var problemExists = await _dbContext.Problems
            .AnyAsync(p => p.Id == request.ProblemId && p.IsActive, cancellationToken);

        if (!problemExists)
        {
            return Result.Failure(ProblemStatusCodes.ProblemNotFound);
        }

        var alreadyViewed = await _dbContext.ProblemViews
            .AnyAsync(v => v.ProblemId == request.ProblemId && v.ViewerId == profileId.Value && v.IsActive, cancellationToken);

        if (alreadyViewed)
        {
            return Result.Success(ProblemStatusCodes.ViewRecorded);
        }

        var view = ProblemView.Create(
            problemId: request.ProblemId,
            viewerId: profileId.Value,
            deviceType: request.DeviceType,
            ipHash: request.IpHash,
            country: request.Country);

        _viewRepo.Add(view);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProblemStatusCodes.ViewRecorded);
    }
}
