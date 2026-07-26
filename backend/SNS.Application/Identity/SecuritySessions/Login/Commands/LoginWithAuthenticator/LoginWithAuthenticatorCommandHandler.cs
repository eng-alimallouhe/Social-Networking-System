using OtpNet;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySessions.Login.Contracts;
using SNS.Application.Identity.SecuritySessions.Shared.Abstractions;
using SNS.Application.Identity.SecuritySessions.Shared.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.SecuritySessions;
using SNS.Application.Identity.Shared.ValueObjects;
using SNS.Domain.Identity.SecuritySessions.Events;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySessions.Login.Commands.LoginWithAuthenticator;

internal sealed class LoginWithAuthenticatorCommandHandler : ICommandHandler<LoginWithAuthenticatorCommand, LoginInitialResponseDto>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ICodeService _codeService;
    private readonly IRequestInfoService _requestInfoService;
    private readonly IDeviceService _deviceService;
    private readonly ISessionService _sessionService;

    public LoginWithAuthenticatorCommandHandler(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ICodeService codeService,
        IRequestInfoService requestInfoService,
        IDeviceService deviceService,
        ISessionService sessionService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _codeService = codeService;
        _requestInfoService = requestInfoService;
        _deviceService = deviceService;
        _sessionService = sessionService;
    }

    public async Task<Result<LoginInitialResponseDto>> Handle(
        LoginWithAuthenticatorCommand request,
        CancellationToken cancellationToken)
    {
        var identifier = new UserIdentifier(request.UserIdentifier);
        var spec = new UserForLoginSpecification(identifier.Value, identifier.Type);
        var user = await _userRepository.GetSingleAsync(spec, cancellationToken);

        // 2. التحقق من وجود المستخدم
        if (user is null)
        {
            return Result<LoginInitialResponseDto>.Failure(UserStatusCodes.NotFound);
        }

        if (user is null)
        {
            return Result<LoginInitialResponseDto>.Failure(UserStatusCodes.NotFound);
        }

        bool isCodeValid = false;

        // التأكد من أن المستخدم يمتلك مفتاح مصادقة بالفعل
        if (!string.IsNullOrEmpty(user.UserSecuritySettings.AuthenticatorSecretKey))
        {
            try
            {
                var secretBytes = Base32Encoding.ToBytes(user.UserSecuritySettings.AuthenticatorSecretKey);

                var totp = new Totp(secretBytes);

                isCodeValid = totp.VerifyTotp(request.Code, out long timeStepMatched, new VerificationWindow(previous: 1, future: 1));
            }
            catch (FormatException)
            {
                isCodeValid = false;
            }
        }

        if (!isCodeValid)
        {
            user.IncrementFailedLoginAttempts();
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result<LoginInitialResponseDto>.Failure(VerificationStatusCodes.InvalidCode);
        }

        user.ResetFailedLoginAttempts();

        user.ResetFailedLoginAttempts();

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
            return Result<LoginInitialResponseDto>.Failure(sessionsResult.StatusCode);
        }

        var recipientAddress = user.UserSecuritySettings.DefaultCommunicationMethod switch
        {
            CommunicationMethod.RecoveryEmail when !string.IsNullOrEmpty(user.UserSecuritySettings.RecoveryEmail) => user.UserSecuritySettings.RecoveryEmail,
            CommunicationMethod.Email when !string.IsNullOrEmpty(user.Email) => user.Email,
            _ => user.Email
        };

        user.AddDomainEvent(new UserLoggedInEvent(
            UserId: user.Id,
            SessionId: sessionsResult.Value.SessionId,
            IpAddress: requestInformationModel.IpAddress,
            Device: requestInformationModel.DeviceName,
            City: requestInformationModel.City,
            Country: requestInformationModel.Country,
            Latitude: requestInformationModel.Latitude,
            Longitude: requestInformationModel.Longitude,
            UserLanguage: user.PreferredLanguage,
            RecipientAddress: recipientAddress,
            SendMethod: user.UserSecuritySettings.DefaultCommunicationMethod,
            OccurredOn: DateTime.UtcNow));

        var accessToken = _tokenService.GenerateAccessToken(new AccessTokenCreateDto(
            UserId: user.Id,
            ProfileId: user.UserProfile.Id,
            RoleType: user.Role.Type,
            SessionId: sessionsResult.Value.SessionId));

        if (accessToken == null)
        {
            return Result<LoginInitialResponseDto>.Failure(SecurityStatusCodes.TokenGenerationError);
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<LoginInitialResponseDto>.Success(new LoginInitialResponseDto(
            UserId: user.Id,
            DeviceId: device.deviceId,
            AccessToken: accessToken,
            RefreshToken: sessionsResult.Value.RefreshToken), OperationStatusCode.Success);
    }
}