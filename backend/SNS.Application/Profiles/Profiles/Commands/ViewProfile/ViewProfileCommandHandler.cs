using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Profiles.Profiles.Commands.ViewProfile;

internal sealed class ViewProfileCommandHandler : ICommandHandler<ViewProfileCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ISoftDeletableRepository<ProfileView> _profileViewRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ViewProfileCommandHandler(
        ICurrentUserService currentUserService,
        ISoftDeletableRepository<ProfileView> profileViewRepo,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _profileViewRepo = profileViewRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ViewProfileCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;

        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (request.ViewedProfileId == profileId.Value)
        {
            return Result.Failure(OperationStatusCode.Conflict);
        }


        var isViewed = await _profileViewRepo
            .ExistsAsync(pv => pv.ViewerId == profileId.Value && pv.ViewedId == request.ViewedProfileId, cancellationToken);


        if (isViewed)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        _profileViewRepo.Add(ProfileView.Create(profileId.Value, request.ViewedProfileId));
        
        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success(OperationStatusCode.Success);
    }
}