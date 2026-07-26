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

namespace SNS.Application.Identity.Users.Registeration.Commands.VerifyUser;

public sealed class VerifyUserCommandHandler : ICommandHandler<VerifyUserCommand, AuthTokensDto>
{
    private readonly ICodeService _codeService;
    private readonly IRepository<User> _userRepo;
    private readonly ISessionService _sessionService;
    private readonly IArchiveService _archiveService;
    private readonly IRequestInfoService _requestInfoService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDeviceService _deviceService;
    private readonly ITokenService _tokenService;

    public VerifyUserCommandHandler(
        ICodeService codeService, IDeviceService deviceService,
        IRepository<User> userRepo, ISessionService sessionService,
        ITokenService tokenService, IRequestInfoService requestInfoService,
        IArchiveService archiveService, IUnitOfWork unitOfWork)
    {
        _codeService = codeService;
        _userRepo = userRepo; 
        _deviceService = deviceService;
        _sessionService = sessionService;
        _archiveService = archiveService;
        _requestInfoService = requestInfoService; 
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthTokensDto>> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
    {
        var spec = new UserForRegisterationSpecification(request.UserId);
        var user = await _userRepo.GetSingleAsync(spec, cancellationToken);

        if (user == null)
            return Result<AuthTokensDto>.Failure(UserStatusCodes.NotFound);

        var verifyResult = await _codeService.VerifyCodeAsync(new VerifyCodeDto(
            UserId: user.Id, 
            Code: request.Code, 
            CodeType: CodeType.AccountActivation, 
            Token: request.Token), cancellationToken);

        if (verifyResult.IsFailure)
            return Result<AuthTokensDto>.Failure(verifyResult.StatusCode);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            user.Verify();


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
                    UserId: request.UserId,
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

            // 9. توليد الـ Access Token والـ Refresh Token النهائي
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
                ActionType: ActionType.AccountActivated, 
                PerformedBy: user.Id, 
                Parameters: new Dictionary<ReplacementKey, string>
                {
                    { ReplacementKey.Device, requestInformationModel.DeviceName },
                    { ReplacementKey.IpAddress, requestInformationModel.IpAddress },
                    {ReplacementKey.Browser, requestInformationModel.Browser  }
                }), cancellationToken);

            await _archiveService.ArchiveIdentityAsync(
                new CreateIdentityArchiveDto(
                    UserId: user.Id, 
                    OldIdentifier: "N/A", 
                    NewIdentifier: user.Email, 
                    IdentityType: IdentityType.Email), cancellationToken);

            await _archiveService.ArchivePasswordAsync(userId: user.Id, cancellationToken);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result<AuthTokensDto>.Success(new AuthTokensDto(accessToken, sessionResult.Value.RefreshToken), OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
