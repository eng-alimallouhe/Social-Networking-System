using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.ChangeUserName;

public sealed class ChangeUserNameCommandHandler : ICommandHandler<ChangeUserNameCommand>
{
    private readonly IApplicationDbContext _dbContext; // للقراءة فقط بأعلى أداء 🔎
    private readonly IRepository<User> _userRepo; // للكتابة والتتبع الحركي 🏗️
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserCacheService _userCacheService; // لتحديث كاش الهوية فوراً

    public ChangeUserNameCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<User> userRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IUserCacheService userCacheService)
    {
        _dbContext = dbContext;
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _userCacheService = userCacheService;
    }

    public async Task<Result> Handle(ChangeUserNameCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null || userId == Guid.Empty)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        string cleanedUserName = request.NewUserName.Trim().ToLowerInvariant(); 

        var isUserNameTaken = await _dbContext.Users
            .AnyAsync(u => u.UserName == cleanedUserName && u.Id != userId, cancellationToken);

        if (isUserNameTaken)
        {
            return Result.Failure(UserStatusCodes.UserNameAlreadyExists);
        }

        var user = await _userRepo.GetByIdAsync(userId.Value, cancellationToken);
        
        if (user == null)
        {
            return Result.Failure(UserStatusCodes.NotFound);
        }

        if (user.UserName == cleanedUserName)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        user.ChangeUserName(cleanedUserName);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _userCacheService.RemoveUserAsync(user.Id, cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}