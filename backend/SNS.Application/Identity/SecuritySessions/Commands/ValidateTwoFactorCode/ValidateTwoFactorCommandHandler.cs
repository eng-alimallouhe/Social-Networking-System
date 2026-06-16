using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Identity.SecuritySessions.Abstractions;
using SNS.Application.Identity.SecuritySessions.DTOs;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Archives;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.SecuritySessions;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Domain.Identity.ArchiveManagement.Enums;
using SNS.Domain.Identity.SecuritySessions.Events;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySessions.Commands.ValidateTwoFactorCode;

public sealed class ValidateTwoFactorCommandHandler : ICommandHandler<ValidateTwoFactorCommand, AuthTokensDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<User> _userRepo;
    private readonly IRequestInfoService _requestInfoService;
    private readonly IAuthResponseService _authResponseService;
    private readonly ICodeService _codeService;
    private readonly IArchiveService _archiveService;
    private readonly ISessionService _userSessionService;
    private readonly IDeviceService _deviceService;

    public ValidateTwoFactorCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<User> userRepo,
        IRequestInfoService requestInfoService,
        IAuthResponseService authResponseService,
        ICodeService codeService,
        IArchiveService archiveService,
        ISessionService userSessionService,
        IDeviceService deviceService)
    {
        _unitOfWork = unitOfWork;
        _userRepo = userRepo;
        _authResponseService = authResponseService; 
        _requestInfoService = requestInfoService;
        _codeService = codeService;
        _archiveService = archiveService;
        _userSessionService = userSessionService;
        _deviceService = deviceService;
    }

    public async Task<Result<AuthTokensDto>> Handle(ValidateTwoFactorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var spec = new UserWithRoleAndSettingsSpecification(request.UserId);

            var user = await _userRepo.GetSingleAsync(spec, cancellationToken);

            if (user == null)
            {
                return Result<AuthTokensDto>.Failure(UserStatusCodes.NotFound);
            }

            if (!user.UserSecuritySettings.IsMfaEnabled)
            {
                return Result<AuthTokensDto>.Failure(OperationStatusCode.Conflict);
            }
            

            var verifyCodeDto = new VerifyCodeDto(
                UserId: request.UserId, 
                Code: request.Code,
                Token: request.Token,
                CodeType: CodeType.LoginTwoFactor);

            var codeVerifyResult = await _codeService.VerifyCodeAsync(verifyCodeDto, cancellationToken);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            if (codeVerifyResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<AuthTokensDto>.Failure(codeVerifyResult.StatusCode);
            }

            var deviceName = _requestInfoService.DeviceName;
            var city = _requestInfoService.City;
            var country = _requestInfoService.Country;

            var device = await _deviceService.GetOrCreateUserDeviceAsync(
                new DeviceCreateDto(
                    UserId: user.Id,
                    DeviceToken: _requestInfoService.DeviceToken,
                    FriendlyName: _requestInfoService.DeviceName,
                    Browser: _requestInfoService.Browser,
                    OperatingSystem: _requestInfoService.OperatingSystem,
                    DeviceVendor: _requestInfoService.DeviceVendor,
                    DeviceModel: _requestInfoService.DeviceModel,
                    FingerprintHash: _requestInfoService.FingerprintHash,
                    IsTrusted: false));

            var sessionArgs = new CreateSessionDto(
                UserId: user.Id,
                DeviceId: device.deviceId,
                IpAddress: _requestInfoService.IpAddress,
                City: _requestInfoService.City,
                Country: _requestInfoService.Country,
                Longitude: _requestInfoService.Longitude,
                Latitude: _requestInfoService.Latitude,
                Browser: _requestInfoService.Browser,
                FingerprintHash: _requestInfoService.FingerprintHash,
                IsDeviceTrusted: device.isDeviceTrusted);

            var sessionResult = await _userSessionService.CreateSessionAsync(sessionArgs);

            if (sessionResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<AuthTokensDto>.Failure(sessionResult.StatusCode);
            }

            var authResult = await _authResponseService.GenerateAuthResponseAsync(
                new AuthResponseGenerationDto(user.Id, user.RoleId, sessionResult.Value, user.Role.Type),
                cancellationToken);


            if (authResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return authResult;
            }

            var createArchiveDto = new CreateUserArchiveDto(
                UserId: user.Id,
                ActionType: ActionType.Login, 
                PerformedBy: user.Id,
                Parameters: new Dictionary<ReplacementKey, string>
                {
                    { ReplacementKey.Device, deviceName }, { ReplacementKey.City, city }, { ReplacementKey.Country, country }
                });

            await _archiveService.LogUserActionAsync(createArchiveDto, cancellationToken);

            if (user.UserSecuritySettings.LoginAlerts)
            {
                var recipientAddress = user.UserSecuritySettings.DefaultCommunicationMethod switch
                {
                    CommunicationMethod.RecoveryEmail => user.UserSecuritySettings.RecoveryEmail!,
                    CommunicationMethod.Email => user.Email,
                    _ => user.Email
                };

                user.AddDomainEvent(new UserLoggedInEvent(
                    UserId: user.Id,
                    SessionId: sessionResult.Value,
                    IpAddress: sessionArgs.IpAddress, 
                    DeviceName: deviceName,
                    UserLanguage: user.PreferredLanguage,
                    SendMethod: user.UserSecuritySettings.DefaultCommunicationMethod,
                    RecipientAddress: recipientAddress,
                    Country: country,
                    City: city,
                    Latitude: sessionArgs.Latitude,
                    Longitude: sessionArgs.Longitude,
                    OccurredOn: DateTime.UtcNow));
            }

            await _unitOfWork.CompleteAsync(cancellationToken);
            
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            return authResult;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
