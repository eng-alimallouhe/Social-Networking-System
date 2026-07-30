using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Profiles.SocialGraph.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Profiles;

namespace SNS.Application.Profiles.SocialGraph.Commands.MuteProfile;

/// <summary>
/// Handles the execution of <see cref="MuteProfileCommand"/> to mute a followed profile.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves current authenticated profile ID.
/// 2. Fetches the existing follow relationship with the target profile.
/// 3. Updates mute state on the follow relationship entity with the specified time period.
/// 4. Persists changes to database.
/// Side effects include follow relationship entity property update and database persistence.
/// </remarks>
internal sealed class MuteProfileCommandHandler
    : ICommandHandler<MuteProfileCommand>
{
    private readonly IRepository<Follow> _followRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public MuteProfileCommandHandler(
        IRepository<Follow> followRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _followRepo = followRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }


    public async Task<Result> Handle(MuteProfileCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(ProfileStatusCodes.NotFound);
        }

        var folloingRelationship = await _followRepo.GetSingleByExpressionAsync(f => f.FollowerId == profileId.Value && f.FollowingId == request.TargetProfileId, cancellationToken);

        if (folloingRelationship == null)
        {
            return Result.Success(ProfileStatusCodes.RelationNotFound);
        }

        folloingRelationship.Mute(request.Period);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}