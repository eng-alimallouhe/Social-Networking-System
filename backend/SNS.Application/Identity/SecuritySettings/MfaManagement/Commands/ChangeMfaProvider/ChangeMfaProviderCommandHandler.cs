using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.ChangeMfaProvider;

public sealed class ChangeMfaProviderCommandHandler : ICommandHandler<ChangeMfaProviderCommand>
{
    private readonly IRepository<UserSecuritySettings> _userSecuritySettings; // التتبع والكتابة من الـ Root الرئيسي للـ Aggregate 🏗️
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ChangeMfaProviderCommandHandler(
        IRepository<UserSecuritySettings> userSecuritySettings,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _userSecuritySettings = userSecuritySettings;
        _unitOfWork = unitOfWork;
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
            return Result.Success(OperationStatusCode.Success);
        }

        userSecuritySettings.ChangeMfaProvider(request.NewProvider);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}