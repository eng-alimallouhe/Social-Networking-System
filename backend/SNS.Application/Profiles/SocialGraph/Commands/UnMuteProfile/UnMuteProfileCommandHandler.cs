using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Profiles.SocialGraph.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Profiles;

namespace SNS.Application.Profiles.SocialGraph.Commands.UnMuteProfile;

/// <summary>
/// Handles the execution of <see cref="UnMuteProfileCommand"/> to unmute a followed profile.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated profile ID.
/// 2. Fetches the existing follow relationship entity.
/// 3. Clears mute state on the follow relationship entity and persists changes.
/// Side effects include follow relationship property update and database persistence.
/// </remarks>
internal class UnMuteProfileCommandHandler :
    ICommandHandler<UnMuteProfileCommand>
{
    private readonly IRepository<Follow> _followRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UnMuteProfileCommandHandler(
    IRepository<Follow> followRepo,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork)
    {
        _followRepo = followRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }


    public async Task<Result> Handle(UnMuteProfileCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(ProfileStatusCodes.NotFound);
        }

        var folloingRelationship = await _followRepo.GetSingleByExpressionAsync(f => f.FollowerId == profileId.Value && f.FollowingId == request.TargetProfileId, cancellationToken);

        if (folloingRelationship == null)
        {
            return Result.Failure(ProfileStatusCodes.RelationNotFound);
        }

        if (!folloingRelationship.IsMuted)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        folloingRelationship.UnMute();

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}