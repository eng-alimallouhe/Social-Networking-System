using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.RemoveAuthenticatorApp;


public sealed record RemoveAuthenticatorAppCommand() : ICommand;

internal class RemoveAuthenticatorAppCommandHandler
    : ICommandHandler<RemoveAuthenticatorAppCommand>
{
    private readonly IRepository<UserSecuritySettings> _userSecuritySettingsRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RemoveAuthenticatorAppCommandHandler(
        IRepository<UserSecuritySettings> userSecuritySettingsRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _userSecuritySettingsRepo = userSecuritySettingsRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(RemoveAuthenticatorAppCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var userSecuritySettings = await _userSecuritySettingsRepo.GetSingleByExpressionAsync(us => us.UserId == userId.Value, cancellationToken);

        if (userSecuritySettings == null)
        {
            return Result.Failure(UserStatusCodes.NotFound);
        }

        if (string.IsNullOrEmpty(userSecuritySettings.AuthenticatorSecretKey))
        {
            return Result.Success(OperationStatusCode.Success);
        }

        userSecuritySettings.RemoveAuthenticator();

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success(OperationStatusCode.Success);
    }
}
