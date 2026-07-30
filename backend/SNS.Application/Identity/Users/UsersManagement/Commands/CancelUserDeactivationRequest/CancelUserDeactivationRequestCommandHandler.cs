using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Identity.SecuritySessions.Shared.Abstractions;
using SNS.Application.Identity.SecuritySessions.Shared.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Archives;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.SecuritySessions;
using SNS.Domain.Identity.ArchiveManagement.Enums;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Identity.Users.Events;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.CancelUserDeactivationRequest;

/// <summary>
/// Handles the execution of <see cref="CancelUserDeactivationRequestCommand"/> to reactivate a deactivated user account.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Validates user existence, deactivation status, and grace period window.
/// 2. Verifies the user activation challenge token using <see cref="IUserCacheService"/>.
/// 3. Re-activates the user entity state.
/// 4. Publishes <see cref="UserActivatedSynchronousEvent"/> and <see cref="UserActivatedIntegrationEvent"/> domain events.
/// 5. Registers/fetches user device, creates a new security session, and generates access and refresh tokens.
/// 6. Logs account activation in the activity archive and completes the activation challenge.
/// Side effects include entity activation, domain event publishing, session creation, user activity archiving, cache state clearance, and database transaction commit.
/// </remarks>
public sealed class CancelUserDeactivationRequestCommandHandler : ICommandHandler<CancelUserDeactivationRequestCommand, AuthTokensDto>
{
    private readonly IRepository<User> _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestInfoService _requestInfoService;
    private readonly IHashingService _hashingService;
    private readonly ITokenService _tokenService;
    private readonly IArchiveService _archiveService;
    private readonly ISessionService _sessionService;
    private readonly IDeviceService _deviceService;
    private readonly IUserCacheService _userCacheService;


    public CancelUserDeactivationRequestCommandHandler(
        IRepository<User> userRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IRequestInfoService requestInfoService,
        IHashingService hashingService,
        ITokenService tokenService,
        IArchiveService archiveService,
        ISessionService sessionService,
        IDeviceService deviceService,
        IUserCacheService userCacheService)
    {
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _requestInfoService = requestInfoService;
        _hashingService = hashingService;
        _tokenService = tokenService;
        _archiveService = archiveService;
        _sessionService = sessionService;
        _deviceService = deviceService;
        _userCacheService = userCacheService;
    }

    public async Task<Result<AuthTokensDto>> Handle(CancelUserDeactivationRequestCommand request, CancellationToken cancellationToken)
    {
        var spec = new UserWithRoleAndSettingsAndProfileSpecification(request.UserId);
        var user = await _userRepo.GetSingleAsync(spec, cancellationToken);

        if (user == null)
        {
            return Result<AuthTokensDto>.Failure(UserStatusCodes.NotFound);
        }

    
        if (user.Status != UserStatus.Deactivated)
        {
            return Result<AuthTokensDto>.Failure(OperationStatusCode.Conflict);
        }

        if (user.Status == UserStatus.Deactivated && user.DeactivatedAt.HasValue && user.DeactivatedAt.Value.AddDays(60) <= DateTime.UtcNow)
        {
            return Result<AuthTokensDto>.Failure(UserStatusCodes.Deactivated);
        }

        var verifyChallageResult = await _userCacheService.VerifyUserActivationChanlageAsync(request.UserId, request.Token, cancellationToken);

        if (verifyChallageResult.IsFailure)
        {
            return Result<AuthTokensDto>.Failure(UserStatusCodes.FailedLoginAttempt);
        }

        var requestInformationModel = new RequestInformationModel(
        IpAddress: _requestInfoService.IpAddress,
        Country: _requestInfoService.Country,
        DeviceName: _requestInfoService.DeviceName,
        Browser: _requestInfoService.Browser,
        Longitude: _requestInfoService.Longitude,
        Latitude: _requestInfoService.Latitude,
        City: _requestInfoService.City,
        DeviceId: _requestInfoService.DeviceId,
        FingerprintHash: _requestInfoService.FingerprintHash,
        OperatingSystem: _requestInfoService.OperatingSystem,
        DeviceToken: _requestInfoService.DeviceToken,
        DeviceModel: _requestInfoService.DeviceModel,
        DeviceVendor: _requestInfoService.DeviceVendor);


        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            user.Activate();

            var now = DateTime.UtcNow;

            user.AddDomainEvent(new UserActivatedSynchronousEvent(
                UserId: user.Id,
                ActivatedAt: now,
                Device: _requestInfoService.DeviceName,
                Browser: _requestInfoService.Browser,
                Country: _requestInfoService.Country,
                IpAddress: _requestInfoService.IpAddress,
                OccurredOn: now));
            
            user.AddDomainEvent(new UserActivatedIntegrationEvent(
                UserId: user.Id,
                ActivatedAt: now,
                Device: _requestInfoService.DeviceName,
                Browser: _requestInfoService.Browser,
                Country: _requestInfoService.Country,
                City: _requestInfoService.City,
                IpAddress: _requestInfoService.IpAddress,
                OccurredOn: now));

            var device = await _deviceService.GetOrCreateUserDeviceAsync(new DeviceCreateDto(
            UserId: user.Id,
            FriendlyName: requestInformationModel.DeviceName,
            FingerprintHash: requestInformationModel.FingerprintHash,
            DeviceToken: requestInformationModel.DeviceToken,
            Browser: requestInformationModel.Browser,
            OperatingSystem: requestInformationModel.OperatingSystem,
            DeviceModel: requestInformationModel.DeviceModel,
            DeviceVendor: requestInformationModel.DeviceVendor,
            IsTrusted: false));

            var sessionsResult = await _sessionService.CreateSessionAsync(new CreateSessionDto(
                UserId: user.Id,
                DeviceId: device.deviceId,
                IpAddress: requestInformationModel.IpAddress,
                City: requestInformationModel.City,
                Country: requestInformationModel.Country,
                FingerprintHash: requestInformationModel.FingerprintHash,
                Browser: requestInformationModel.Browser,
                Longitude: requestInformationModel.Longitude,
                Latitude: requestInformationModel.Latitude,
                IsDeviceTrusted: device.isDeviceTrusted));

            if (sessionsResult.IsFailure || sessionsResult.Value == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<AuthTokensDto>.Failure(sessionsResult.StatusCode);
            }

            var accessToken = _tokenService.GenerateAccessToken(
                new AccessTokenCreateDto(
                    UserId: user.Id,
                    ProfileId: user.UserProfile.Id,
                    SessionId: sessionsResult.Value.SessionId,
                    RoleType: user.Role.Type));

            if (accessToken == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<AuthTokensDto>.Failure(SecurityStatusCodes.TokenGenerationError);
            }

            await _archiveService.LogUserActionAsync(new CreateUserArchiveDto(
                UserId: user.Id,
                ActionType: ActionType.AccountActivated,
                PerformedBy: user.Id,
                Parameters: new Dictionary<ReplacementKey, string>
                {
                    { ReplacementKey.Device, requestInformationModel.DeviceName },
                    { ReplacementKey.Browser, requestInformationModel.Browser }
                }), cancellationToken);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await _userCacheService.CompleteUserActivationChanlageAsync(user.Id, cancellationToken);

            return Result<AuthTokensDto>.Success(new AuthTokensDto(accessToken, sessionsResult.Value.RefreshToken), OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
