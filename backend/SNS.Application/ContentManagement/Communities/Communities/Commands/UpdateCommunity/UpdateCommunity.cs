using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.Contracts.Storage;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.ContentManagement.Communities.Events;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Communities.Communities.Commands.UpdateCommunity;

/// <summary>
/// Represents a command to update an existing community's details, type, status, or logo.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
/// <param name="Name">The updated name of the community.</param>
/// <param name="Description">The updated description of the community.</param>
/// <param name="RulesText">The updated rules summary text.</param>
/// <param name="Policy">The updated moderation policy.</param>
/// <param name="Type">The updated privacy/visibility type.</param>
/// <param name="Status">The updated community status.</param>
/// <param name="Logo">Optional new logo file to replace the existing one.</param>
public sealed record UpdateCommunityCommand(
    Guid CommunityId,
    string Name,
    string Description,
    string RulesText,
    ModerationPolicy Policy,
    CommunityType Type,
    CommunityStatus Status,
    UploadedFile? Logo = null
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="UpdateCommunityCommand"/> to update community information.
/// </summary>
internal sealed class UpdateCommunityCommandHandler : ICommandHandler<UpdateCommunityCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Community> _communityRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMediator _mediator;

    public UpdateCommunityCommandHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Community> communityRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        IMediator mediator)
    {
        _dbContext = dbContext;
        _communityRepo = communityRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateCommunityCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var community = await _communityRepo.GetByIdAsync(request.CommunityId, cancellationToken);
        if (community == null || !community.IsActive)
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

        if (community.Name != request.Name)
        {
            var nameExists = await _dbContext.Communities
                .AnyAsync(c => c.Name == request.Name && c.Id != request.CommunityId && c.IsActive, cancellationToken);

            if (nameExists)
            {
                return Result.Failure(OperationStatusCode.Conflict);
            }
        }

        string? logoObjectKey = null;
        if (request.Logo != null && request.Logo.Length > 0)
        {
            logoObjectKey = $"communities/logos/{Guid.NewGuid():N}.{request.Logo.Extension}";
            await _fileStorageService.UploadFileAsync(
                fileStream: request.Logo.Stream,
                objectKey: logoObjectKey,
                contentType: request.Logo.ContentType,
                cancellationToken: cancellationToken);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            community.Update(
                name: request.Name,
                description: request.Description,
                rulesText: request.RulesText,
                policy: request.Policy,
                type: request.Type,
                status: request.Status,
                logoObjectKey: logoObjectKey);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await _mediator.Publish(
                new DomainEventNotification<CommunityUpdatedIntegrationEvent>(
                    new CommunityUpdatedIntegrationEvent(community.Id, DateTime.UtcNow)),
                cancellationToken);

            return Result.Success(OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
