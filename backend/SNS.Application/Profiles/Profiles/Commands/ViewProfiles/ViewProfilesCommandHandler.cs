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