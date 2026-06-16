using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.DisableMFA;

public sealed class DisableMFACommandHandler : ICommandHandler<DisableMFACommand>
{
    private readonly IRepository<UserSecuritySettings> _userSecuritySettingsRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DisableMFACommandHandler(
        IRepository<UserSecuritySettings> userSecuritySettingsRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _userSecuritySettingsRepo = userSecuritySettingsRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DisableMFACommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return Result.Failure(OperationStatusCode.AuthenticationRequired);
        }

        var userSettings = await _userSecuritySettingsRepo.GetSingleByExpressionAsync(
            us => us.UserId == userId);

        if (userSettings == null)
        {
            return Result.Failure(UserStatusCodes.NotFound);
        }

        if (!userSettings.IsMfaEnabled)
        {
            return Result.Failure(OperationStatusCode.Conflict);
        }

        userSettings.DisableMfa();

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}
