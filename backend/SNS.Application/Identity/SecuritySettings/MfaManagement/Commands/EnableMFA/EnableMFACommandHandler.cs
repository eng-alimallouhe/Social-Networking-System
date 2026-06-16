using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.EnableMFA;

public sealed class EnableMFACommandHandler : ICommandHandler<EnableMFACommand>
{
    private readonly IRepository<UserSecuritySettings> _userSecuritySettingsRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public EnableMFACommandHandler(
        IRepository<UserSecuritySettings> userSecuritySettingsRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _userSecuritySettingsRepo = userSecuritySettingsRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(EnableMFACommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null || userId == Guid.Empty)
        {
            return Result.Failure(OperationStatusCode.AuthenticationRequired);
        }

        var userSettings = await _userSecuritySettingsRepo.GetSingleByExpressionAsync(
            us => us.UserId == userId.Value, cancellationToken);

        if (userSettings == null)
        {
            return Result.Failure(UserStatusCodes.NotFound);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            userSettings.EnableMfa(mfaProvider: request.MfaProvider);

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
