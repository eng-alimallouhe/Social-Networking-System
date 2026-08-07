using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Identity.SecuritySettings.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Exceptions;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.ChangeMfaProvider;

public sealed class ChangeMfaProviderCommandHandler : ICommandHandler<ChangeMfaProviderCommand>
{
    private readonly IRepository<UserSecuritySettings> _userSecuritySettings; // التتبع والكتابة من الـ Root الرئيسي للـ Aggregate 🏗️
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public ChangeMfaProviderCommandHandler(
        IRepository<UserSecuritySettings> userSecuritySettings,
        IUnitOfWork unitOfWork,
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _userSecuritySettings = userSecuritySettings;
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ChangeMfaProviderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        
        if (userId == null || userId == Guid.Empty)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var userSecuritySettings = await _userSecuritySettings.GetSingleByExpressionAsync(
            uss => uss.UserId == userId, cancellationToken);

        if (userSecuritySettings == null)
        {
            return Result.Failure(UserStatusCodes.NotFound);
        }

        if (userSecuritySettings.MfaProvider == request.NewProvider)
        {
        }

        if (request.NewProvider == MfaProvider.RecoveryEmail && string.IsNullOrEmpty(userSecuritySettings.RecoveryEmail))
        {
            return Result.Failure(SecurityStatusCodes.RecoveryEmailNotLinked);
        }
        else if (request.NewProvider == MfaProvider.AuthenticatorApp && string.IsNullOrEmpty(userSecuritySettings.AuthenticatorSecretKey))
        {
            return Result.Failure(SecurityStatusCodes.AuthenticatorAppNotLinked);
        }
        else if (request.NewProvider == MfaProvider.Passkey)
        {
            var anyPasskey = await _dbContext
                .UserPasskeys
                .AnyAsync(
                ups => ups.UserId == userId, cancellationToken);

            if (!anyPasskey)
            {
                return Result.Failure(SecurityStatusCodes.PasskeyNotAdded);
            }
        }

        userSecuritySettings.ChangeMfaProvider(request.NewProvider);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}