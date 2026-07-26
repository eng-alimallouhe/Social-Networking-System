using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Profiles.Profiles.Commands.UpdateSocialLinks;

internal sealed class UpdateSocialLinksCommandHandler : ICommandHandler<UpdateSocialLinksCommand>
{
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateSocialLinksCommandHandler(
        ISoftDeletableRepository<Profile> profileRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _profileRepo = profileRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateSocialLinksCommand request, CancellationToken cancellationToken)
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

        profile.UpdateSocialLinks(
            gitHubUrl: request.GitHubUrl, 
            linkedInUrl: request.LinkedInUrl, 
            facebookUrl: request.FaceBookUrl, 
            website: request.Website, 
            xUrl: request.XUrl);

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success(OperationStatusCode.Success);
    }
}