using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Profiles.Profiles.Commands.UpdateBasicInformation;

/// <summary>
/// Handles the execution of <see cref="UpdateBasicInformationCommand"/> to update profile details.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Fetches profile entity from repository.
/// 3. Updates full name, bio, specialization, and location properties on the profile.
/// 4. Persists profile modifications to database.
/// Side effects include profile entity state update and database persistence.
/// </remarks>
internal sealed record UpdateBasicInformationCommandHandler: 
    ICommandHandler<UpdateBasicInformationCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IUnitOfWork _unitOfWork;


    public UpdateBasicInformationCommandHandler(
        ICurrentUserService currentUserService,
        ISoftDeletableRepository<Profile> profileRepo,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _profileRepo = profileRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateBasicInformationCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;

        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var profile = await _profileRepo.GetByIdAsync(profileId.Value, cancellationToken);

        if (profile == null)
        {
            return Result.Failure(UserStatusCodes.ProfileNotCompleted);
        }

        profile.UpdateBasicInformation(
            fullName: request.FullName,
            bio: request.Bio,
            specialization: request.Specialization,
            location: request.Location);

        await _unitOfWork.CompleteAsync(cancellationToken);
        
        return Result.Success(OperationStatusCode.Success);
    }
}