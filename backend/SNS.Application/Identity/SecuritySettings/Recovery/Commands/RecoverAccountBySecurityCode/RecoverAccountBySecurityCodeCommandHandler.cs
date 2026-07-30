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
using SNS.Domain.Identity.SecuritySessions.Events;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.Recovery.Commands.RecoverAccountBySecurityCode;

/// <summary>
/// Handles the execution of <see cref="RecoverAccountBySecurityCodeCommand"/> to recover a user account using a security code.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Hashes the security code and verifies the matching user.
/// 2. Clears previous active sessions for the user.
/// 3. Registers or fetches the device details and initiates a new security session.
/// 4. Generates new access and refresh authentication tokens.
/// 5. Logs user action into the user activity archive.
/// 6. Raises <see cref="UserLoggedInBySecurityCodeEvent"/> if login alerts are active.
/// Side effects include session clearance, session creation, user action archiving, domain event publication, and transactional state persistence.
/// </remarks>
public sealed class RecoverAccountBySecurityCodeCommandHandler : ICommandHandler<RecoverAccountBySecurityCodeCommand, AuthTokensDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<User> _userRepo;
    private readonly IHashingService _hashingService;
    private readonly ITokenService _tokenService;
    private readonly IArchiveService _archiveService;
    private readonly IRequestInfoService _requestInfoService;
    private readonly ISessionService _sessionService;
    private readonly IDeviceService _deviceService;

    public RecoverAccountBySecurityCodeCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<User> userRepo,
        IHashingService hashingService,
        ITokenService tokenService,
        IArchiveService archiveService,
        IRequestInfoService requestInfoService,
        ISessionService sessionService,
        IDeviceService deviceService)
    {
        _unitOfWork = unitOfWork;
        _userRepo = userRepo;
        _sessionService = sessionService;
        _hashingService = hashingService;
        _deviceService = deviceService;
        _tokenService = tokenService;
        _archiveService = archiveService;
        _requestInfoService = requestInfoService;
    }

    public async Task<Result<AuthTokensDto>> Handle(RecoverAccountBySecurityCodeCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var codeHash = _hashingService.Hash(request.SecurityCode);

            var spec = new UserBySecurityCodeSpecification(codeHash);
            
            var user = await _userRepo.GetSingleAsync(spec, cancellationToken);

            if (user == null) 
                return Result<AuthTokensDto>.Failure(UserStatusCodes.NotFound);

            await _sessionService.ClearSessionsByUserIdAsync(user.Id, cancellationToken);

            var requestInformationModel = new RequestInformationModel(
                IpAddress: _requestInfoService.IpAddress,
                Country: _requestInfoService.Country,
                City: _requestInfoService.City,
                Latitude: _requestInfoService.Latitude,
                Longitude: _requestInfoService.Longitude,
                DeviceId: _requestInfoService.DeviceId,
                Browser: _requestInfoService.Browser,
                DeviceName: _requestInfoService.DeviceName,
                DeviceModel: _requestInfoService.DeviceModel,
                DeviceVendor: _requestInfoService.DeviceVendor,
                FingerprintHash: _requestInfoService.FingerprintHash,
                DeviceToken: _requestInfoService.DeviceToken,
                OperatingSystem: _requestInfoService.OperatingSystem);


            var device = await _deviceService.GetOrCreateUserDeviceAsync(new DeviceCreateDto(
                UserId: user.Id,
                DeviceToken: requestInformationModel.DeviceToken,
                FriendlyName: requestInformationModel.DeviceName,
                Browser: requestInformationModel.Browser,
                OperatingSystem: requestInformationModel.OperatingSystem,
                DeviceVendor: requestInformationModel.DeviceVendor,
                DeviceModel: requestInformationModel.DeviceModel,
                FingerprintHash: requestInformationModel.FingerprintHash,
                IsTrusted: false));

            // 8. إنشاء الجلسة الأمنية (Security Session) عبر خدمة الجلسات
            var sessionResult = await _sessionService.CreateSessionAsync(
                new CreateSessionDto(
                    UserId: user.Id,
                    DeviceId: device.deviceId,
                    IpAddress: _requestInfoService.IpAddress,
                    City: _requestInfoService.City,
                    Country: _requestInfoService.Country,
                    Latitude: _requestInfoService.Latitude,
                    Longitude: _requestInfoService.Longitude,
                    FingerprintHash: _requestInfoService.FingerprintHash,
                    Browser: _requestInfoService.Browser,
                    IsDeviceTrusted: device.isDeviceTrusted), cancellationToken);

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

            await _archiveService.LogUserActionAsync(new CreateUserArchiveDto(
                UserId: user.Id,
                ActionType: ActionType.Login, 
                PerformedBy: user.Id, 
                Parameters: new Dictionary<ReplacementKey, string>
                {
                    {  ReplacementKey.Device, requestInformationModel.DeviceName  },
                    { ReplacementKey.IpAddress, requestInformationModel.IpAddress },
                    { ReplacementKey.City, requestInformationModel.City },
                    { ReplacementKey.Country, requestInformationModel.Country } 
                }));

            var recipientAddress = user.UserSecuritySettings.DefaultCommunicationMethod == CommunicationMethod.RecoveryEmail?
                user.UserSecuritySettings.RecoveryEmail! : user.Email;

            if (user.UserSecuritySettings.LoginAlerts)
            {
                user.AddDomainEvent(new UserLoggedInBySecurityCodeEvent(
                    UserId: user.Id,
                    SessionId: sessionResult.Value.SessionId,
                    IpAddress: requestInformationModel.IpAddress,
                    Device: requestInformationModel.DeviceName,
                    SendLanguage: user.PreferredLanguage,
                    RecipientAddress: recipientAddress,
                    City: requestInformationModel.City,
                    Country: requestInformationModel.Country,
                    Latitude: requestInformationModel.Latitude,
                    Longitude: requestInformationModel.Longitude,
                    SendMethod: user.UserSecuritySettings.DefaultCommunicationMethod,
                    OccurredOn: DateTime.UtcNow));
            }

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result<AuthTokensDto>.Success(new AuthTokensDto(Token: accessToken, RefreshToken: sessionResult.Value.RefreshToken), OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
