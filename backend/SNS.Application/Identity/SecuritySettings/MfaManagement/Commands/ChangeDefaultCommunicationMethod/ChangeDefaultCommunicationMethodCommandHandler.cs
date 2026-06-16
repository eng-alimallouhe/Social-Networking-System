using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.ChangeDefaultCommunicationMethod;

public sealed class ChangeDefaultCommunicationMethodCommandHandler : ICommandHandler<ChangeDefaultCommunicationMethodCommand>
{
    private readonly IRepository<UserSecuritySettings> _userSecuritySettingsRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeDefaultCommunicationMethodCommandHandler(
        IRepository<UserSecuritySettings> userSecuritySettingsRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {

        _userSecuritySettingsRepo = userSecuritySettingsRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeDefaultCommunicationMethodCommand request, CancellationToken cancellationToken)
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

        if (userSettings.DefaultCommunicationMethod == request.NewCommunicationMethod)
        {
            return Result.Failure(OperationStatusCode.Conflict);
        }

        userSettings.ChangeDefaultCommunicationMethod(method: request.NewCommunicationMethod);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}
