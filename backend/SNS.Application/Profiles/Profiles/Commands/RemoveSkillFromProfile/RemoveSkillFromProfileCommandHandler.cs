using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Profiles.Profiles.Relations;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Profiles.Profiles.Commands.RemoveSkillFromProfile;

/// <summary>
/// Handles the execution of <see cref="RemoveSkillFromProfileCommand"/> to remove a skill from a user profile.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Fetches the target profile-skill association.
/// 3. Removes the profile-skill entity and persists database changes.
/// Side effects include entity deletion from database.
/// </remarks>
internal sealed record RemoveSkillFromProfileCommandHandler
    : ICommandHandler<RemoveSkillFromProfileCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<ProfileSkill> _profileSkillRepo;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveSkillFromProfileCommandHandler(
        ICurrentUserService currentUserService,
        IRepository<ProfileSkill> profileSkillRepo,
        IUnitOfWork unitOfWork)

    {
        _currentUserService = currentUserService;
        _profileSkillRepo = profileSkillRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveSkillFromProfileCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;

        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var skill = await _profileSkillRepo.GetByIdAsync(request.SkillId, cancellationToken);

        if (skill == null)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        _profileSkillRepo.Delete(skill);

        await _unitOfWork.CompleteAsync(cancellationToken);
        
        return Result.Success(OperationStatusCode.Success);
    }
}