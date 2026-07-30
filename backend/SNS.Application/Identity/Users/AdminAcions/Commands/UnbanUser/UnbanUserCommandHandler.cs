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

namespace SNS.Application.Identity.Users.AdminAcions.Commands.UnbanUser;

/// <summary>
/// Handles the execution of <see cref="UnbanUserCommand"/> to restore a banned user account.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Verifies that the performing user has administrative role authorization.
/// 2. Fetches the target user entity and checks that their status is currently permanently banned.
/// 3. Reverts the user status to active via unban logic.
/// 4. Dispatches a <see cref="UserUnBannedEvent"/> domain event.
/// 5. Commits changes within a database transaction.
/// Side effects include status update, domain event publishing, and database transaction commit.
/// </remarks>
public sealed class UnbanUserCommandHandler : ICommandHandler<UnbanUserCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<User> _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;


    public UnbanUserCommandHandler(
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

    public async Task<Result> Handle(UnbanUserCommand request, CancellationToken cancellationToken)
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

        if (targetUser.Status != UserStatus.PermanentlyBanned)
        {
            return Result.Failure(OperationStatusCode.Conflict);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            targetUser.UnBan();

            targetUser.AddDomainEvent(new UserUnBannedEvent(
                UserId: targetUser.Id,
                Email: targetUser.UserSecuritySettings.DefaultCommunicationMethod == CommunicationMethod.Email ?
                            targetUser.UserSecuritySettings.RecoveryEmail! : targetUser.Email,
                UserLanguage: targetUser.PreferredLanguage,
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