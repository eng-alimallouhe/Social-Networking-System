using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Profiles.Profiles.Commands.ViewProfiles;

/// <summary>
/// Handles the execution of <see cref="ViewProfilesCommand"/> to record batch profile view events.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated viewer profile ID.
/// 2. Queries existing profile view records to avoid duplicates.
/// 3. Filters unrecorded profile IDs and constructs new <see cref="ProfileView"/> entities.
/// 4. Adds new entities and saves changes to database.
/// Side effects include batch creation of profile view records in persistence store.
/// </remarks>
internal sealed class ViewProfilesCommandHandler : ICommandHandler<ViewProfilesCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISoftDeletableRepository<ProfileView> _profileViewRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ViewProfilesCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        ISoftDeletableRepository<ProfileView> profileViewRepo,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _profileViewRepo = profileViewRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ViewProfilesCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;

        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }


        var existingViews = await _dbContext
            .ProfileViews
            .Where(pv => pv.ViewerId == profileId.Value && request.ViewedProfileIds.Contains(pv.ViewedId))
            .Select(pv => pv.ViewedId)
            .ToHashSetAsync(cancellationToken);

        var viewsList = request.ViewedProfileIds
            .Where(viewedId => !existingViews.Contains(viewedId))
            .Select(viewedId => ProfileView.Create(profileId.Value, viewedId))
            .Distinct()
            .ToList();

        _profileViewRepo.AddRange(viewsList);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}