using OtpNet;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.CompleteAuthenticatorRegistration;

public class CompleteAuthenticatorRegistrationCommandHandler
    : ICommandHandler<CompleteAuthenticatorRegistrationCommand>
{
    private readonly IRepository<UserSecuritySettings> _securitySettingsRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserCacheService _userCacheService;

    public CompleteAuthenticatorRegistrationCommandHandler(
        IRepository<UserSecuritySettings> securitySettingsRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IUserCacheService userCacheService)
    {
        _securitySettingsRepo = securitySettingsRepository;
        _unitOfWork = unitOfWork;
        _userCacheService = userCacheService;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        CompleteAuthenticatorRegistrationCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null || userId == Guid.Empty)
        {
            return Result.Failure(OperationStatusCode.AuthenticationRequired);
        }

        var securitySettings = await _securitySettingsRepo
            .GetSingleByExpressionAsync(u => u.UserId == userId.Value, cancellationToken);

        var secretKey = await _userCacheService.GetAuthenticatorSecretKeyAsync(userId.Value, cancellationToken);

        if (securitySettings == null || string.IsNullOrEmpty(secretKey))
        {
            return Result.Failure(OperationStatusCode.Conflict);
        }

        byte[] secretBytes;
        try
        {
            secretBytes = Base32Encoding.ToBytes(secretKey);
        }
        catch (Exception)
        {
            return Result.Failure(OperationStatusCode.ServerError);
        }

        // Create a TOTP instance using the secret key
        var totp = new Totp(secretBytes);

        // Set the verification window to allow for clock skew: this allows for a 30-second window before and after the current time step
        var verificationWindow = new VerificationWindow(previous: 1, future: 1);

        bool isCodeValid = totp.VerifyTotp(
            request.Code,
            out long timeStepMatched,
            verificationWindow);

        if (!isCodeValid)
        {
            // The provided code is invalid
            return Result.Failure(SecurityStatusCodes.InvalidMfaCode);
        }

        // Begin a transaction to update the security settings
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Enable the authenticator for the user
            securitySettings.EnableAuthenticator(secretKey);

            // Save the changes to the database
            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success(OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
