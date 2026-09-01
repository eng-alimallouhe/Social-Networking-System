using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Communities.Settings.Commands.UpdateCommunitySettings;

/// <summary>
/// Represents a command to update configuration settings for a community.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
/// <param name="AllowPostWithoutApproval">Whether posts require moderation approval before appearing.</param>
/// <param name="AllowInvitationsByMembers">Whether members can invite new users.</param>
/// <param name="AllowComments">Whether post commenting is enabled.</param>
/// <param name="AllowMediaUpload">Whether media uploads are permitted.</param>
public sealed record UpdateCommunitySettingsCommand(
    Guid CommunityId,
    bool AllowPostWithoutApproval,
    bool AllowInvitationsByMembers,
    bool AllowComments,
    bool AllowMediaUpload
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="UpdateCommunitySettingsCommand"/> to modify community settings.
/// </summary>
internal sealed class UpdateCommunitySettingsCommandHandler : ICommandHandler<UpdateCommunitySettingsCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<SNS.Domain.ContentManagement.Communities.Entities.CommunitySettings> _settingsRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCommunitySettingsCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<SNS.Domain.ContentManagement.Communities.Entities.CommunitySettings> settingsRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _settingsRepo = settingsRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateCommunitySettingsCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var community = await _dbContext.Communities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CommunityId && c.IsActive, cancellationToken);

        if (community == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        var isOwner = community.OwnerId == profileId.Value;
        var isModerator = !isOwner && await _dbContext.CommunityMemberships
            .AnyAsync(m => m.CommunityId == request.CommunityId &&
                           m.MemberId == profileId.Value &&
                           (m.Role == CommunityRole.Moderator || m.Role == CommunityRole.Owner) &&
                           m.Status == CommunityMembershipStatus.Active, cancellationToken);

        if (!isOwner && !isModerator)
        {
            return Result.Failure(SecurityStatusCodes.UnAuthorized);
        }

        var settings = await _settingsRepo.GetSingleByExpressionAsync(s => s.CommunityId == request.CommunityId, cancellationToken);
        if (settings == null)
        {
            settings = SNS.Domain.ContentManagement.Communities.Entities.CommunitySettings.Create(
                request.CommunityId,
                request.AllowPostWithoutApproval,
                request.AllowInvitationsByMembers,
                request.AllowComments,
                request.AllowMediaUpload);
            _settingsRepo.Add(settings);
        }
        else
        {
            settings.Update(
                request.AllowPostWithoutApproval,
                request.AllowInvitationsByMembers,
                request.AllowComments,
                request.AllowMediaUpload);
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}
