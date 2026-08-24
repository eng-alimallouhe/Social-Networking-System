using SNS.Application.Abstractions.Common;
using SNS.Application.Identity.SecuritySessions.Shared.Abstractions;
using SNS.Application.Identity.SecuritySessions.Shared.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.SecuritySessions;
using SNS.Application.Identity.Shared.DTOs.Users;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Shared.Services;

public class AuthenticationFlowService
    : IAuthenticationFlowService
{
    private readonly ITokenService _tokenService;
    private readonly IRequestInfoService _requestInfoService;
    private readonly ISessionService _sessionService;
    private readonly IRepository<Device> _deviceRepo;

    public AuthenticationFlowService(
        ITokenService tokenService,
        IRequestInfoService requestInfoService,
        ISessionService sessionService,
        IRepository<Device> deviceRepo)
    {
        _tokenService = tokenService;
        _sessionService = sessionService;
        _requestInfoService = requestInfoService;
        _deviceRepo = deviceRepo;
    }

    public async Task<Result<AuthTokensDto>> AuthenticateUserAsync(
    AuthenticateUserRequest authenticateUserRequest,
    CancellationToken cancellationToken = default)
    {
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

        Guid sessionId;
        string refreshToken;

        // ============================================================
        // FLOW 1:
        // Existing Session
        // ============================================================

        if (authenticateUserRequest.SessionId.HasValue)
        {
            var refreshTokenResult = await _sessionService.RotateRefreshTokenAsync(
                sessionId: authenticateUserRequest.SessionId.Value,
                userId: authenticateUserRequest.UserId,
                cancellationToken: cancellationToken);

            if (refreshTokenResult.IsFailure || refreshTokenResult.Value == null)
            {
                return Result<AuthTokensDto>.Failure(
                    refreshTokenResult.StatusCode);
            }

            sessionId = authenticateUserRequest.SessionId.Value;
            refreshToken = refreshTokenResult.Value;
        }

        // ============================================================
        // FLOW 2:
        // No Session -> Find/Create Device -> Create Session
        // ============================================================

        else
        {
            var device = await _deviceRepo.GetSingleByExpressionAsync(
                d =>
                    d.UserId == authenticateUserRequest.UserId &&
                    (
                        d.DeviceToken == requestInformationModel.DeviceToken ||
                        d.FingerprintHash == requestInformationModel.FingerprintHash
                    ),
                cancellationToken);

            if (device == null)
            {
                device = Device.Create(
                    userId: authenticateUserRequest.UserId,
                    deviceToken: requestInformationModel.DeviceToken,
                    friendlyName: requestInformationModel.DeviceName,
                    browser: requestInformationModel.Browser,
                    operatingSystem: requestInformationModel.OperatingSystem,
                    deviceModel: requestInformationModel.DeviceModel,
                    deviceVendor: requestInformationModel.DeviceVendor,
                    fingerprintHash: requestInformationModel.FingerprintHash,
                    isTrusted: false);

                _deviceRepo.Add(device);
            }

            var sessionResult = await _sessionService.CreateSessionAsync(
                new CreateSessionDto(
                    UserId: authenticateUserRequest.UserId,
                    DeviceId: device.Id,
                    IpAddress: requestInformationModel.IpAddress,
                    City: requestInformationModel.City,
                    Country: requestInformationModel.Country,
                    FingerprintHash: requestInformationModel.FingerprintHash,
                    Browser: requestInformationModel.Browser,
                    Longitude: requestInformationModel.Longitude,
                    Latitude: requestInformationModel.Latitude,
                    IsDeviceTrusted: device.IsTrusted),
                cancellationToken);

            if (sessionResult.IsFailure ||
                sessionResult.Value == null)
            {
                return Result<AuthTokensDto>.Failure(
                    sessionResult.StatusCode);
            }

            sessionId = sessionResult.Value.SessionId;
            refreshToken = sessionResult.Value.RefreshToken;
        }

        // ============================================================
        // Generate Access Token
        // ============================================================

        var accessToken = _tokenService.GenerateAccessToken(
            new AccessTokenCreateDto(
                UserId: authenticateUserRequest.UserId,
                ProfileId: authenticateUserRequest.ProfileId,
                SessionId: sessionId,
                RoleType: authenticateUserRequest.RoleType));

        if (accessToken == null)
        {
            return Result<AuthTokensDto>.Failure(
                SecurityStatusCodes.TokenGenerationError);
        }

        return Result<AuthTokensDto>.Success(
            new AuthTokensDto(
                Token: accessToken,
                RefreshToken: refreshToken),
            OperationStatusCode.Success);
    }
}