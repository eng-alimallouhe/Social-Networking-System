using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.ContentManagement.Communities.Rules.Contracts;
using SNS.Application.ContentManagement.Communities.Settings.Contracts;
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

namespace SNS.Application.ContentManagement.Communities.Communities.Commands.CreateCommunity;

/// <summary>
/// Represents a command to create a new community with initial settings and rules.
/// </summary>
/// <param name="Name">The unique name of the community.</param>
/// <param name="Description">The description of the community.</param>
/// <param name="RulesText">The summary text or guidelines of community rules.</param>
/// <param name="Policy">The moderation policy of the community.</param>
/// <param name="Type">The privacy/visibility type of the community.</param>
/// <param name="Logo">Optional uploaded logo file.</param>
/// <param name="Settings">Optional initial community configuration settings.</param>
/// <param name="Rules">Optional initial structured rules list.</param>
public sealed record CreateCommunityCommand(
    string Name,
    string Description,
    string RulesText,
    ModerationPolicy Policy,
    CommunityType Type,
    UploadedFile? Logo = null,
    CommunitySettingsDto? Settings = null,
    List<CreateCommunityRuleDto>? Rules = null
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="CreateCommunityCommand"/> to create a new community entity.
/// </summary>
internal sealed class CreateCommunityCommandHandler : ICommandHandler<CreateCommunityCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Community> _communityRepo;
    private readonly IRepository<CommunityMembership> _membershipRepo;
    private readonly IRepository<SNS.Domain.ContentManagement.Communities.Entities.CommunitySettings> _settingsRepo;
    private readonly IRepository<CommunityRule> _ruleRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMediator _mediator;

    public CreateCommunityCommandHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Community> communityRepo,
        IRepository<CommunityMembership> membershipRepo,
        IRepository<SNS.Domain.ContentManagement.Communities.Entities.CommunitySettings> settingsRepo,
        IRepository<CommunityRule> ruleRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        IMediator mediator)
    {
        _dbContext = dbContext;
        _communityRepo = communityRepo;
        _membershipRepo = membershipRepo;
        _settingsRepo = settingsRepo;
        _ruleRepo = ruleRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
        _mediator = mediator;
    }

    public async Task<Result> Handle(CreateCommunityCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result.Failure(OperationStatusCode.InvalidInput);
        }

        var nameExists = await _dbContext.Communities
            .AnyAsync(c => c.Name == request.Name && c.IsActive, cancellationToken);

        if (nameExists)
        {
            return Result.Failure(OperationStatusCode.Conflict);
        }

        string logoObjectKey = string.Empty;
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
            var community = Community.Create(
                ownerId: profileId.Value,
                name: request.Name,
                description: request.Description,
                rulesText: request.RulesText,
                policy: request.Policy,
                type: request.Type,
                status: CommunityStatus.Active,
                logoUrl: logoObjectKey);

            _communityRepo.Add(community);

            var ownerMembership = CommunityMembership.Create(
                memberId: profileId.Value,
                communityId: community.Id,
                role: CommunityRole.Owner,
                status: CommunityMembershipStatus.Active);
            _membershipRepo.Add(ownerMembership);

            var settings = SNS.Domain.ContentManagement.Communities.Entities.CommunitySettings.Create(
                communityId: community.Id,
                allowPostWithoutApproval: request.Settings?.AllowPostWithoutApproval ?? true,
                allowInvitationsByMembers: request.Settings?.AllowInvitationsByMembers ?? true,
                allowComments: request.Settings?.AllowComments ?? true,
                allowMediaUpload: request.Settings?.AllowMediaUpload ?? true);
            _settingsRepo.Add(settings);

            if (request.Rules != null && request.Rules.Any())
            {
                foreach (var rule in request.Rules)
                {
                    var ruleEntity = CommunityRule.Create(community.Id, rule.Title, rule.Description, rule.Order);
                    _ruleRepo.Add(ruleEntity);
                }
            }

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await _mediator.Publish(
                new DomainEventNotification<CommunityCreatedIntegrationEvent>(
                    new CommunityCreatedIntegrationEvent(community.Id, DateTime.UtcNow)),
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
