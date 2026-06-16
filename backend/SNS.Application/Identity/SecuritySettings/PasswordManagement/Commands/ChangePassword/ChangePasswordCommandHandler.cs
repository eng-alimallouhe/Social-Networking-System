using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Identity.SecuritySessions.Abstractions;
using SNS.Application.Identity.SecuritySessions.DTOs;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Archives;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.SecuritySessions;
using SNS.Domain.Identity.ArchiveManagement.Enums;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, AuthTokensDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDeviceService _deviceService;
    private readonly IAuthResponseService _authResponseService;
    private readonly IRequestInfoService _requestInfoService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<User> _userRepo;
    private readonly IHashingService _hashingService;
    private readonly IArchiveService _archiveService;
    private readonly ISessionService _sessionService;

    public ChangePasswordCommandHandler(
        IUnitOfWork unitOfWork,
        IDeviceService deviceService,
        IAuthResponseService authResponseService,
        IRequestInfoService requestInfoService,
        ICurrentUserService currentUserService,
        IRepository<User> userRepo,
        IHashingService hashingService,
        IArchiveService archiveService,
        ISessionService sessionService)
    {
        _unitOfWork = unitOfWork;
        _deviceService = deviceService;
        _authResponseService = authResponseService;
        _requestInfoService = requestInfoService;
        _currentUserService = currentUserService;
        _userRepo = userRepo;
        _hashingService = hashingService;
        _archiveService = archiveService;
        _sessionService = sessionService;
    }

    public async Task<Result<AuthTokensDto>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        
        var userId = _currentUserService.UserId;

        if (userId == null) 
            return Result<AuthTokensDto>.Failure(OperationStatusCode.AuthenticationRequired);

        var spec = new UserWithRoleAndSettingsSpecification(userId.Value);

        var user = await _userRepo.GetSingleAsync(spec, cancellationToken);
        
        if (user == null) 
            return Result<AuthTokensDto>.Failure(UserStatusCodes.NotFound);

        if (!_hashingService.Verify(request.CurrentPassword, user.PasswordHash))
        {
            await Task.Delay(Random.Shared.Next(100, 300), cancellationToken);
            return Result<AuthTokensDto>.Failure(OperationStatusCode.Failure);
        }

        if (request.NewPassword == request.CurrentPassword)
            return Result<AuthTokensDto>.Failure(OperationStatusCode.Conflict);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
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


            user.ChangePassword(hashedPassword: _hashingService.Hash(request.NewPassword));

            await _sessionService.ClearSessionsByUserIdAsync(user.Id, cancellationToken);

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

            var sessionArgs = new CreateSessionDto(
                user.Id,
                DeviceId: device.deviceId, 
                IpAddress: requestInformationModel.IpAddress, 
                City: requestInformationModel.City,
                Country: requestInformationModel.Country,
                FingerprintHash: requestInformationModel.FingerprintHash,
                Browser: requestInformationModel.Browser, 
                Longitude: requestInformationModel.Longitude,
                Latitude: requestInformationModel.Latitude,
                IsDeviceTrusted: device.isDeviceTrusted);

            var sessionResult = await _sessionService.CreateSessionAsync(sessionArgs, cancellationToken);

            if (sessionResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<AuthTokensDto>.Failure(sessionResult.StatusCode);
            }

            var authResult = await _authResponseService.GenerateAuthResponseAsync(
                new AuthResponseGenerationDto(user.Id, user.RoleId, sessionResult.Value, user.Role.Type)
                , cancellationToken);

            if (authResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync (cancellationToken);
                return Result<AuthTokensDto>.Failure(authResult.StatusCode);
            }

            await _archiveService.LogUserActionAsync(
                new CreateUserArchiveDto(
                    UserId: user.Id, 
                    ActionType: ActionType.PasswordChanged, 
                    PerformedBy: user.Id, 
                    Parameters: new Dictionary<ReplacementKey, string>
                    {
                        { ReplacementKey.City, requestInformationModel.City},
                        { ReplacementKey.Country, requestInformationModel.Country},
                        { ReplacementKey.Device, requestInformationModel.DeviceName},
                        
                    }), cancellationToken);

            await _archiveService.ArchivePasswordAsync(user.Id, cancellationToken);

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
