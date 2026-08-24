using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Identity.SecuritySessions.Shared.Abstractions;
using SNS.Application.Identity.SecuritySessions.Shared.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Archives;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.SecuritySessions;
using SNS.Application.Identity.Shared.DTOs.Users;
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

/// <summary>
/// Handles the execution of <see cref="VerifyUserCommand"/> to verify an account and issue initial authentication tokens.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Verifies the provided activation code and token via <see cref="ICodeService"/>.
/// 2. Updates user state to verified.
/// 3. Registers device details and creates an initial security session.
/// 4. Generates access and refresh authentication tokens.
/// 5. Logs account activation, identity, and password entries in the user archive.
/// Side effects include updating user status, session creation, user activity archiving, and transaction commitment.
/// </remarks>
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
    private readonly IAuthenticationFlowService _authenticationFlowService;

    public VerifyUserCommandHandler(
        ICodeService codeService, IDeviceService deviceService,
        IRepository<User> userRepo, ISessionService sessionService,
        ITokenService tokenService, IRequestInfoService requestInfoService,
        IArchiveService archiveService, IUnitOfWork unitOfWork,
        IAuthenticationFlowService authenticationFlowService)
    {
        _codeService = codeService;
        _userRepo = userRepo; 
        _deviceService = deviceService;
        _sessionService = sessionService;
        _archiveService = archiveService;
        _requestInfoService = requestInfoService; 
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _authenticationFlowService = authenticationFlowService;
    }

    public async Task<Result<AuthTokensDto>> Handle(
    VerifyUserCommand request,
    CancellationToken cancellationToken)
    {
        var spec = new UserForRegisterationSpecification(request.UserId);

        var user = await _userRepo.GetSingleAsync(
            spec,
            cancellationToken);

        if (user == null)
            return Result<AuthTokensDto>.Failure(
                UserStatusCodes.NotFound);

        var verifyResult = await _codeService.VerifyCodeAsync(
            new VerifyCodeDto(
                UserId: user.Id,
                Code: request.Code,
                CodeType: CodeType.AccountActivation,
                Token: request.Token),
            cancellationToken);

        if (verifyResult.IsFailure)
            return Result<AuthTokensDto>.Failure(
                verifyResult.StatusCode);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            user.Verify();

            var authTokens =
                await _authenticationFlowService.AuthenticateUserAsync(
                    new AuthenticateUserRequest(
                        UserId: user.Id,
                        RoleId: user.RoleId,
                        RoleType: user.Role.Type,
                        ProfileId: null,
                        SessionId: null),
                    cancellationToken);

            if (authTokens.IsFailure || authTokens.Value == null)
            {
                await _unitOfWork.RollbackTransactionAsync(
                    cancellationToken);

                return authTokens;
            }

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

            await _archiveService.LogUserActionAsync(
                new CreateUserArchiveDto(
                    UserId: user.Id,
                    ActionType: ActionType.AccountCreated,
                    PerformedBy: user.Id,
                    Parameters: new Dictionary<ReplacementKey, string>
                    {
                    {
                        ReplacementKey.Device,
                        requestInformationModel.DeviceName
                    },
                    {
                        ReplacementKey.IpAddress,
                        requestInformationModel.IpAddress
                    },
                    {
                        ReplacementKey.Browser,
                        requestInformationModel.Browser
                    }
                    }),
                cancellationToken);

            await _archiveService.ArchiveIdentityAsync(
                new CreateIdentityArchiveDto(
                    UserId: user.Id,
                    OldIdentifier: "N/A",
                    NewIdentifier: user.Email,
                    IdentityType: IdentityType.Email),
                cancellationToken);

            await _archiveService.ArchivePasswordAsync(
                userId: user.Id,
                cancellationToken);

            await _unitOfWork.CompleteAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(
                cancellationToken);

            return Result<AuthTokensDto>.Success(
                authTokens.Value,
                OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(
                cancellationToken);

            throw;
        }
    }
}
