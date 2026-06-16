using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Events;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.AdminAcions.Commands.ChangeUserRole;

public sealed class ChangeUserRoleCommandHandler : ICommandHandler<ChangeUserRoleCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<User> _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ChangeUserRoleCommandHandler(
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

    public async Task<Result> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var userRole = _currentUserService.RoleType;
            
        if (userId == null || userRole == null || !userRole.Contains("admin", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }
            
        var targetUser = await _userRepo.GetByIdAsync(request.TargetUserId, cancellationToken);

        if (targetUser== null)
        {
            return Result.Failure(UserStatusCodes.NotFound);
        }

        Guid? targetRoleId = await _dbContext
            .Roles
            .Where(r => r.Type == request.NewRole)
            .Select(r => r.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (targetRoleId == null || targetRoleId == Guid.Empty)
            return Result.Failure(SecurityStatusCodes.RoleNotFound);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            targetUser.ChangeRole(newRoleId: targetRoleId.Value);

            targetUser.AddDomainEvent(new UserRoleChangedEvent(
                UserId: targetUser.Id,
                RoleName: request.NewRole.ToString(),
                Email: targetUser.Email,
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
