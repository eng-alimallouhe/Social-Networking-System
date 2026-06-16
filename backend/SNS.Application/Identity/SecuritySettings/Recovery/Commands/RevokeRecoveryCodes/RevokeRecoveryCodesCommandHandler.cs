using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.Recovery.Commands.RevokeRecoveryCodes;

public sealed class RevokeRecoveryCodesCommandHandler : ICommandHandler<RevokeRecoveryCodesCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<RecoveryCode> _recoveryCodeRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RevokeRecoveryCodesCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<RecoveryCode> recoveryCodeRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _recoveryCodeRepo = recoveryCodeRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        RevokeRecoveryCodesCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null || userId == Guid.Empty)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var securitySettingsId = await _dbContext.UsersSecuritySettings
            .Where(ss => ss.UserId == userId)
            .Select(ss => ss.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (securitySettingsId == Guid.Empty)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        await _recoveryCodeRepo.ExecuteDeleteAsync(rc => rc.UserSecuritySettingsId == securitySettingsId, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}