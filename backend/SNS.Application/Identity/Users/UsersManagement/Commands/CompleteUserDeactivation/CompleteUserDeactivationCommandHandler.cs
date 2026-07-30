using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySessions.Shared.Abstractions;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Events;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.CompleteUserDeactivation;

/// <summary>
/// Handles the execution of <see cref="CompleteUserDeactivationCommand"/> to complete account deactivation.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Verifies the deactivation code and token via <see cref="ICodeService"/>.
/// 2. Sets user status to deactivated and soft-deletes the user's profile entity.
/// 3. Raises a <see cref="UserDeactivatedEvent"/> domain event.
/// 4. Commits changes within a database transaction.
/// 5. Evicts user and profile caches, and revokes all active security sessions for the user.
/// Side effects include user deactivation, profile soft-deletion, domain event publishing, session clearance, cache eviction, and transaction persistence.
/// </remarks>
public sealed class CompleteUserDeactivationCommandHandler : ICommandHandler<CompleteUserDeactivationCommand>
{
    private readonly ICodeService _codeService;
    private readonly IRepository<User> _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserCacheService _userCacheService;
    private readonly IProfileCacheService _profileCacheService;
    private readonly ISessionService _sessionService;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IRequestInfoService _requestInfoService;

    public CompleteUserDeactivationCommandHandler(
        ICodeService codeService,
        IRepository<User> userRepo,
        ISoftDeletableRepository<Profile> profileRepo,
        IUnitOfWork unitOfWork,
        IUserCacheService userCacheService,
        IProfileCacheService profileCacheService,
        ISessionService sessionService,
        IRequestInfoService requestInfoService)
    {
        _codeService = codeService;
        _userRepo = userRepo;
        _profileRepo = profileRepo;
        _unitOfWork = unitOfWork;
        _userCacheService = userCacheService;
        _profileCacheService = profileCacheService;
        _sessionService = sessionService;
        _requestInfoService = requestInfoService;
    }

    public async Task<Result> Handle(CompleteUserDeactivationCommand request, CancellationToken cancellationToken)
    {
        var codeVerifyResult = await _codeService.VerifyCodeAsync(new VerifyCodeDto(
            UserId: request.UserId,
            Code: request.Code,
            CodeType: CodeType.UserDeleting,
            Token: request.Token));

        if (codeVerifyResult.IsFailure)
        {
            return Result.Failure(codeVerifyResult.StatusCode);
        }

        var user = await _userRepo.GetByIdAsync(request.UserId, cancellationToken: cancellationToken);

        var profile = await _profileRepo.GetSingleByExpressionAsync(
            p => p.UserId == request.UserId, cancellationToken: cancellationToken);

        if (user == null || profile == null)
        {
            return Result.Failure(UserStatusCodes.NotFound);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

        try
        {
            user.Deactivate();

            profile.SoftDelete();

            user.AddDomainEvent(new UserDeactivatedEvent(
                UserId: user.Id,
                UserName: user.UserName,
                Email: user.Email,
                SendLanguage: user.PreferredLanguage,
                SendMethod: user.UserSecuritySettings.DefaultCommunicationMethod,
                Device: _requestInfoService.DeviceName,
                Browser: _requestInfoService.Browser,
                Country: _requestInfoService.Country,
                City: _requestInfoService.City,
                Longitude: _requestInfoService.Longitude,
                Latitude: _requestInfoService.Latitude,
                IpAddress: _requestInfoService.IpAddress,
                OccurredOn: DateTime.UtcNow));

            await _unitOfWork.CompleteAsync(cancellationToken: cancellationToken);
            
            await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);
            
            await _userCacheService.RemoveUserAsync(user.Id, cancellationToken);
            await _sessionService.ClearSessionsByUserIdAsync(user.Id, cancellationToken);
            await _profileCacheService.RemoveProfileAsync(profile.Id, user.Id, cancellationToken);

            return Result.Success(OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken: cancellationToken);
            throw;
        }
    }
}