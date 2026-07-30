using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Identity.Users.Events;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.AdminAcions.Commands.PermanentlyBanUser;

/// <summary>
/// Handles the execution of <see cref="PermanentlyBanUserCommand"/> to permanently ban a user account.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Validates administrative privileges of the requesting user.
/// 2. Retrieves the target user entity and checks current ban status.
/// 3. Marks the target user status as permanently banned.
/// 4. Dispatches a <see cref="UserBannedEvent"/> domain event.
/// 5. Commits the ban action within a database transaction.
/// Side effects include entity status update, domain event dispatching, and transaction persistence.
/// </remarks>
public sealed class PermanentlyBanUserCommandHandler : ICommandHandler<PermanentlyBanUserCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<User> _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public PermanentlyBanUserCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<User> userRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(PermanentlyBanUserCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var userRole = _currentUserService.RoleType;

        if (userId == null || userRole == null || !userRole.Contains("admin", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var spec = new UserWithRoleAndSettingsAndProfileSpecification(request.TargetUserId);

        var targetUser = await _userRepo.GetSingleAsync(spec, cancellationToken);

        if (targetUser == null)
        {
            return Result.Failure(UserStatusCodes.NotFound);
        }

        if (targetUser.Status == UserStatus.PermanentlyBanned)
        {
            return Result.Failure(OperationStatusCode.Conflict);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            targetUser.PermanentlyBan();

            targetUser.AddDomainEvent(new UserBannedEvent(
                UserId: targetUser.Id,
                UserName: targetUser.UserName,
                Email: targetUser.UserSecuritySettings.DefaultCommunicationMethod == CommunicationMethod.Email? 
                            targetUser.UserSecuritySettings.RecoveryEmail! : targetUser.Email,
                Reason: request.Reason,
                SendLanguage: targetUser.PreferredLanguage,
                CommunicationMethod: targetUser.UserSecuritySettings.DefaultCommunicationMethod,
                OccurredOn: DateTime.UtcNow)); 

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