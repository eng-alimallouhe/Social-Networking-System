using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.SocialGraph.Abstractions;
using SNS.Domain.Profiles.SocialGraph.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Profiles;

namespace SNS.Application.Profiles.SocialGraph.Commands.UnfollowProfile;

/// <summary>
/// Handles the execution of <see cref="UnfollowProfileCommand"/> to unfollow a user profile.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated follower profile ID.
/// 2. Fetches the existing follow relationship entity.
/// 3. Deletes the follow relationship entity and saves changes to database.
/// Side effects include follow entity removal and database persistence.
/// </remarks>
internal sealed class UnfollowProfileCommandHandler
    : ICommandHandler<UnfollowProfileCommand>
{
    private readonly IRepository<Follow> _followRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UnfollowProfileCommandHandler(
        ISocialPolicyService socialPolicyService,
        IRepository<Follow> followRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _followRepo = followRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UnfollowProfileCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(ProfileStatusCodes.NotFound);
        }

        var folloingRelationship = await _followRepo.GetSingleByExpressionAsync(f => f.FollowerId == profileId.Value && f.FollowingId == request.TargetProfileId, cancellationToken);

        if (folloingRelationship == null)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        _followRepo.Delete(folloingRelationship);

        await _unitOfWork.CompleteAsync(cancellationToken);
        
        return Result.Success(OperationStatusCode.Success);
    }
}
