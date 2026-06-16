using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Identity.SecuritySessions.Abstractions;
using SNS.Application.Identity.SecuritySessions.DTOs;
using SNS.Application.Identity.SecuritySessions.Services;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Archives;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.SecuritySessions;
using SNS.Application.Identity.Shared.Services;
using SNS.Domain.Identity.ArchiveManagement.Enums;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, AuthTokensDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<User> _userRepo;
    private readonly IPendingUpdatesService _pendingUpdatesService;
    private readonly IArchiveService _archiveService;
    private readonly IHashingService _hashingService;
    private readonly ISessionService _sessionService;
    private readonly IAuthResponseService _authResponseService;
    private readonly IRequestInfoService _requestInfoService;
    private readonly IDeviceService _deviceService;

    public ResetPasswordCommandHandler(
        IUnitOfWork unitOfWork,
        IPendingUpdatesService pendingUpdatesService,
        IRepository<User> userRepo,
        IArchiveService archiveService,
        IHashingService hashingService,
        ISessionService sessionService,
        IAuthResponseService authResponseService,
        IRequestInfoService requestInfoService,
        IDeviceService deviceService)
    {
        _unitOfWork = unitOfWork;
        _pendingUpdatesService = pendingUpdatesService;
        _userRepo = userRepo;
        _archiveService = archiveService;
        _hashingService = hashingService;
        _sessionService = sessionService;
        _authResponseService = authResponseService;
        _requestInfoService = requestInfoService;
        _deviceService = deviceService;
    }

    public async Task<Result<AuthTokensDto>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var pendingUpdate = await _pendingUpdatesService.GetPasswordUpdateAsync(request.UserId, cancellationToken);

        if (pendingUpdate == null)
            return Result<AuthTokensDto>.Failure(ResourceStatusCode.NotFound);

        if (pendingUpdate.Token != request.Token)
            return Result<AuthTokensDto>.Failure(OperationStatusCode.AccessDenied);

        if (!pendingUpdate.IsVerified)
            return Result<AuthTokensDto>.Failure(OperationStatusCode.AccessDenied);

        var spec = new UserWithRoleAndSettingsSpecification(request.UserId);

        var user = await _userRepo.GetSingleAsync(spec, cancellationToken);

        if (user == null)
            return Result<AuthTokensDto>.Failure(UserStatusCodes.NotFound);

        if (_hashingService.Verify(request.NewPassword, user.PasswordHash))
            return Result<AuthTokensDto>.Failure(OperationStatusCode.Conflict);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _archiveService.ArchivePasswordAsync(user.Id, cancellationToken);

            user.ChangePassword(hashedPassword: _hashingService.Hash(request.NewPassword));

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

            if (sessionResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<AuthTokensDto>.Failure(sessionResult.StatusCode);
            }

            var tokensResult = await _authResponseService.GenerateAuthResponseAsync(
                new AuthResponseGenerationDto(
                    UserId: user.Id,
                    RoleId: user.RoleId,
                    SessionId: sessionResult.Value,
                    RoleType: user.Role.Type), cancellationToken);

            if (tokensResult.IsFailure || tokensResult.Value == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<AuthTokensDto>.Failure(tokensResult.StatusCode);
            }

            await _archiveService.LogUserActionAsync(
                new CreateUserArchiveDto(
                    UserId: user.Id, 
                    ActionType: ActionType.PasswordChanged, 
                    PerformedBy: user.Id, 
                    Parameters: new Dictionary<ReplacementKey, string>
                    {
                        { ReplacementKey.IpAddress, _requestInfoService.IpAddress},
                        { ReplacementKey.Device, _requestInfoService.DeviceName},
                        { ReplacementKey.Country, _requestInfoService.Country},
                    }),
                cancellationToken);

            await _pendingUpdatesService.DeletePasswordUpdateAsync(user.Id, cancellationToken);

            await _unitOfWork.CompleteAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return tokensResult;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
