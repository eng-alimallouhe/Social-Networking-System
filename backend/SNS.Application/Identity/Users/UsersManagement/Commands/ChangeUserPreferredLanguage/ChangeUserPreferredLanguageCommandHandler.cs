using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.ChangeUserPreferredLanguage;

/// <summary>
/// Handles the execution of <see cref="ChangeUserPreferredLanguageCommand"/> to update language preferences.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Verifies authentication of the requesting user identity.
/// 2. Fetches user entity and checks current language preference.
/// 3. Updates user's preferred language setting and saves database changes.
/// 4. Evicts cached user data from <see cref="IUserCacheService"/>.
/// Side effects include entity property modification, database persistence, and cache eviction.
/// </remarks>
public sealed class ChangeUserPreferredLanguageCommandHandler : ICommandHandler<ChangeUserPreferredLanguageCommand>
{
    private readonly IRepository<User> _userRepo; // التزاماً بالقاعدة: الكتابة والتتبع عبر الـ Repo 🏗️
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserCacheService _userCacheService; // لتحديث الكاش فوراً

    public ChangeUserPreferredLanguageCommandHandler(
        IRepository<User> userRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IUserCacheService userCacheService)
    {
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _userCacheService = userCacheService;
    }

    public async Task<Result> Handle(ChangeUserPreferredLanguageCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null || userId == Guid.Empty)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var user = await _userRepo.GetByIdAsync(userId.Value, cancellationToken);
        if (user == null)
        {
            return Result.Failure(UserStatusCodes.NotFound);
        }

        if (user.PreferredLanguage == request.NewLanguage)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        user.ChangePreferredLanguage(request.NewLanguage);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _userCacheService.RemoveUserAsync(user.Id, cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}