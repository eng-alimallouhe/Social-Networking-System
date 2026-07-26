using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Identity.SecuritySessions.Shared.Abstractions;
using SNS.Application.Identity.SecuritySessions.Shared.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Archives;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.SecuritySessions;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Domain.Identity.ArchiveManagement.Enums;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.EmailChange.Commands.VerifyEmailChange;

public sealed class VerifyEmailChangeCommandHandler : ICommandHandler<VerifyEmailChangeCommand, AuthTokensDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPendingUpdatesService _pendingUpdateService;
    private readonly ITokenService _tokenService;
    private readonly IRequestInfoService _requestInfoService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<User> _userRepo;
    private readonly ICodeService _codeService;
    private readonly IArchiveService _archiveService;
    private readonly ISessionService _userSessionService;
    private readonly IDeviceService _deviceService;
    private readonly IGeneratorService _generatorService;

    public VerifyEmailChangeCommandHandler(
        IUnitOfWork unitOfWork,
        IPendingUpdatesService pendingUpdatesService,
        ITokenService tokenService,
        IRequestInfoService requestInfoService,
        ICurrentUserService currentUserService,
        IRepository<User> userRepo,
        ICodeService codeService,
        IArchiveService archiveService,
        ISessionService userSessionService,
        IDeviceService deviceService,
        IGeneratorService generatorService)
    {
        _unitOfWork = unitOfWork;
        _pendingUpdateService = pendingUpdatesService;
        _tokenService = tokenService;
        _requestInfoService = requestInfoService;
        _currentUserService = currentUserService;
        _userRepo = userRepo;
        _codeService = codeService;
        _deviceService = deviceService;
        _archiveService = archiveService;
        _generatorService = generatorService;
        _userSessionService = userSessionService;
    }

    public async Task<Result<AuthTokensDto>> Handle(VerifyEmailChangeCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
            return Result<AuthTokensDto>.Failure(OperationStatusCode.AuthenticationRequired);

        var spec = new UserWithRoleAndSettingsAndProfileSpecification(userId.Value);

        var user = await _userRepo.GetSingleAsync(spec, cancellationToken);

        if (user == null)
            return Result<AuthTokensDto>.Failure(UserStatusCodes.NotFound);

        var pendingUpdate = await _pendingUpdateService.GetEmailUpdateAsync(user.Id, cancellationToken);

        if (pendingUpdate == null)
            return Result<AuthTokensDto>.Failure(OperationStatusCode.ResourceNotFound);

        var verifyCodeDto = new VerifyCodeDto(
            UserId: userId.Value,
            Token: request.Token,
            Code: request.Code, 
            CodeType: CodeType.ChangeEmail);
        
        var codeVerifyResult = await _codeService.VerifyCodeAsync(verifyCodeDto, cancellationToken);

        if (codeVerifyResult.IsFailure)
            return Result<AuthTokensDto>.Failure(codeVerifyResult.StatusCode);


        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var oldIdentifier = user.UserSecuritySettings.RecoveryEmail ?? "Initial";
            var newIdentifier = pendingUpdate.NewEmail;

            user.ChangeEmail(email: newIdentifier);

            await _pendingUpdateService.DeleteEmailUpdateAsync(user.Id);
            await _userSessionService.ClearSessionsByUserIdAsync(userId.Value, cancellationToken);

            var deviceCreateDto = new DeviceCreateDto(
                UserId: user.Id,
                DeviceToken: _requestInfoService.DeviceToken,
                FriendlyName: _requestInfoService.DeviceName,
                Browser: _requestInfoService.Browser,
                OperatingSystem: _requestInfoService.OperatingSystem,
                DeviceVendor: _requestInfoService.DeviceVendor,
                DeviceModel: _requestInfoService.DeviceModel,
                FingerprintHash: _requestInfoService.FingerprintHash,
                IsTrusted: false);

            var device = await _deviceService.GetOrCreateUserDeviceAsync(deviceCreateDto, cancellationToken);

            var sessionArgs = new CreateSessionDto(
                UserId:  user.Id,
                DeviceId: device.deviceId,
                IpAddress: _requestInfoService.IpAddress, 
                Browser: _requestInfoService.Browser, 
                City: _requestInfoService.City,
                Country: _requestInfoService.Country,
                FingerprintHash: _requestInfoService.FingerprintHash,
                Longitude: _requestInfoService.Longitude,
                Latitude: _requestInfoService.Latitude,
                IsDeviceTrusted: device.isDeviceTrusted);

            var sessionCreateResult = await _userSessionService.CreateSessionAsync(sessionArgs, cancellationToken);
            
            if (sessionCreateResult.IsFailure || sessionCreateResult.Value == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<AuthTokensDto>.Failure(sessionCreateResult.StatusCode);
            }

            var accessToken = _tokenService.GenerateAccessToken(
                new AccessTokenCreateDto(
                    UserId: user.Id,
                    ProfileId: user.UserProfile.Id,
                    SessionId: sessionCreateResult.Value.SessionId,
                    RoleType: user.Role.Type));

            var reason = $"The user updated his Email from device: {deviceCreateDto.FriendlyName}";

            await _archiveService.ArchiveIdentityAsync(
                new CreateIdentityArchiveDto(UserId: user.Id, OldIdentifier: oldIdentifier, NewIdentifier: newIdentifier, IdentityType: IdentityType.Email), cancellationToken);

            await _archiveService.LogUserActionAsync(
                new CreateUserArchiveDto(
                    UserId: user.Id, 
                    ActionType: ActionType.EmailChanged, 
                    PerformedBy: user.Id,
                    Parameters: new Dictionary<ReplacementKey, string>
                    {
                        { ReplacementKey.Device, deviceCreateDto.FriendlyName },
                        { ReplacementKey.NewEmail, newIdentifier }
                    },
                    Reason: reason), cancellationToken);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<AuthTokensDto>.Success(new AuthTokensDto(accessToken, sessionCreateResult.Value.RefreshToken), OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
