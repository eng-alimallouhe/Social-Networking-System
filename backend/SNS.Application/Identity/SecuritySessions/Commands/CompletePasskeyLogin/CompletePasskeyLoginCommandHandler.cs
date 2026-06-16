using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Caching;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySessions.Abstractions;
using SNS.Application.Identity.SecuritySessions.DTOs;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.SecuritySessions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;


namespace SNS.Application.Identity.SecuritySessions.Commands.CompletePasskeyLogin;

public class CompletePasskeyLoginCommandHandler
    : ICommandHandler<CompletePasskeyLoginCommand, LoginResponseDto>
{
    private readonly IFido2 _fido2;
    private readonly IDeviceService _deviceService;
    private readonly IRequestInfoService _requestInfoService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionService _sessionService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<Device> _deviceRepo;
    private readonly ICacheService _cache;
    private readonly IAuthResponseService _authResponseService;

    public CompletePasskeyLoginCommandHandler(
        IFido2 fido2,
        IDeviceService deviceService,
        IRequestInfoService requestInfoService,
        ISessionService sessionService,
        IUnitOfWork unitOfWork,
        IApplicationDbContext dbContext,
        ICacheService cache,
        IRepository<Device> deviceRepo,
        IAuthResponseService authResponseService)
    {
        _fido2 = fido2;
        _deviceService = deviceService;
        _dbContext = dbContext;
        _requestInfoService = requestInfoService;
        _sessionService = sessionService;
        _cache = cache;
        _unitOfWork = unitOfWork;
        _deviceRepo = deviceRepo;
        _authResponseService = authResponseService;
    }

    public async Task<Result<LoginResponseDto>> Handle(
        CompletePasskeyLoginCommand request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"fido2_login_options:{request.UserId}";
        AssertionOptions? origOptions = await _cache.GetAsync<AssertionOptions>(cacheKey, cancellationToken);

        if (origOptions == null)
        {
            return Result<LoginResponseDto>.Failure(SessionStatusCodes.NotFound);
        }

        await _cache.RemoveAsync(cacheKey, cancellationToken);

        var storedPasskey = await _dbContext.UserPasskeys
            .FirstOrDefaultAsync(p => p.UserId == request.UserId &&
                                      p.CredentialId == request.AssertionResponse.RawId, cancellationToken);

        if (storedPasskey == null)
        {
            return Result<LoginResponseDto>.Failure(SecurityStatusCodes.VerificationFailed);
        }

        try
        {
            var makeAssertionParams = new MakeAssertionParams
            {
                AssertionResponse = request.AssertionResponse,
                OriginalOptions = origOptions,
                StoredPublicKey = storedPasskey.PublicKey,
                StoredSignatureCounter = storedPasskey.SignatureCounter,
                IsUserHandleOwnerOfCredentialIdCallback = async (args, cancellation) =>
                {
                    bool isOwner = await _dbContext.UserPasskeys
                        .AnyAsync(p => p.UserId == request.UserId && p.CredentialId == args.CredentialId, cancellation);
                    return isOwner;
                }
            };

            VerifyAssertionResult assertionResult = await _fido2.MakeAssertionAsync(
                makeAssertionParams,
                cancellationToken: cancellationToken
            );

            storedPasskey.UpdateCounter(assertionResult.SignCount);

            var authParams = await _dbContext
                .Users
                .Where(u => u.Id == request.UserId)
                .Select(u => new
                {
                    UserId = u.Id,
                    RoleId = u.RoleId,
                    RoleType = u.Role.Type
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (authParams == null)
            {
                return Result<LoginResponseDto>.Failure(UserStatusCodes.NotFound);
            }

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
                UserId: authParams.UserId,
                DeviceToken: requestInformationModel.DeviceToken,
                FriendlyName: requestInformationModel.DeviceName,
                Browser: requestInformationModel.Browser,
                OperatingSystem: requestInformationModel.OperatingSystem,
                DeviceVendor: requestInformationModel.DeviceVendor,
                DeviceModel: requestInformationModel.DeviceModel,
                FingerprintHash: requestInformationModel.FingerprintHash,
                IsTrusted: false));

            // 8. ????? ?????? ??????? (Security Session) ??? ???? ???????
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

            if (sessionResult.IsFailure)
            {
                return Result<LoginResponseDto>.Failure(sessionResult.StatusCode);
            }

            // 9. ????? ??? Access Token ???? Refresh Token ???????
            var tokensResult = await _authResponseService.GenerateAuthResponseAsync(
                new AuthResponseGenerationDto(
                    UserId: authParams.UserId,
                    RoleId: authParams.RoleId,
                    SessionId: sessionResult.Value,
                    RoleType: authParams.RoleType), cancellationToken);

            if (tokensResult.IsFailure || tokensResult.Value == null)
            {
                return Result<LoginResponseDto>.Failure(tokensResult.StatusCode);
            }

            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result<LoginResponseDto>.Success(new LoginResponseDto(
                UserId: authParams.UserId,
                DeviceId: device.deviceId,
                AccessToken: tokensResult.Value.Token,
                RefreshToken: tokensResult.Value.RefreshToken), OperationStatusCode.Success);
        }
        catch (Fido2VerificationException)
        {
            return Result<LoginResponseDto>.Failure(SecurityStatusCodes.VerificationFailed);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
