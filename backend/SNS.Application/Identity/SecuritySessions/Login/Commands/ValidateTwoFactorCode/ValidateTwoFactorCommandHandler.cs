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
using SNS.Domain.Identity.SecuritySessions.Events;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySessions.Login.Commands.ValidateTwoFactorCode;

public sealed class ValidateTwoFactorCommandHandler : ICommandHandler<ValidateTwoFactorCommand, AuthTokensDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<User> _userRepo;
    private readonly IRequestInfoService _requestInfoService;
    private readonly ITokenService _tokenService;
    private readonly ICodeService _codeService;
    private readonly IArchiveService _archiveService;
    private readonly ISessionService _userSessionService;
    private readonly IDeviceService _deviceService;
    private readonly IGeneratorService _generatorService;

    public ValidateTwoFactorCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<User> userRepo,
        IRequestInfoService requestInfoService,
        ITokenService tokenService,
        ICodeService codeService,
        IArchiveService archiveService,
        ISessionService userSessionService,
        IDeviceService deviceService,
        IGeneratorService generatorService)
    {
        _unitOfWork = unitOfWork;
        _userRepo = userRepo;
        _tokenService = tokenService;
        _requestInfoService = requestInfoService;
        _codeService = codeService;
        _archiveService = archiveService;
        _userSessionService = userSessionService;
        _deviceService = deviceService;
        _generatorService = generatorService;
    }

    public async Task<Result<AuthTokensDto>> Handle(ValidateTwoFactorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var spec = new UserWithRoleAndSettingsAndProfileSpecification(request.UserId);

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

            if (sessionResult.IsFailure || sessionResult.Value == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<AuthTokensDto>.Failure(sessionResult.StatusCode);
            }

            var accessToken = _tokenService.GenerateAccessToken(
                new AccessTokenCreateDto(
                    UserId: user.Id,
                    ProfileId: user.UserProfile.Id,
                    SessionId: sessionResult.Value.SessionId,
                    RoleType: user.Role.Type));


            if (accessToken == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<AuthTokensDto>.Failure(SecurityStatusCodes.TokenGenerationError);
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
                    SessionId: sessionResult.Value.SessionId,
                    IpAddress: sessionArgs.IpAddress, 
                    Device: deviceName,
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
            
            return Result<AuthTokensDto>.Success(
                new AuthTokensDto(
                    Token: accessToken, RefreshToken: sessionResult.Value.RefreshToken),
                OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
