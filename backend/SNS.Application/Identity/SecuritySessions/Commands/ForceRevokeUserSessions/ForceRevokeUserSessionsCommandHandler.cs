using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Notifications.Abstractions;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Identity.SecuritySessions.Specifications;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySessions.Commands.ForceRevokeUserSessions;

public sealed class ForceRevokeUserSessionsCommandHandler
    : ICommandHandler<ForceRevokeUserSessionsCommand>
{
    private readonly IApplicationDbContext _dbContext; // للقراءة أو السحب الحركي السريع
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationHubService _hubService; // الخدمة اللحظية التي بنيناها معاً 🔔
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<SecuritySession> _sessionRepo;

    public ForceRevokeUserSessionsCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        INotificationHubService hubService,
        IRepository<User> userRepo,
        IRepository<SecuritySession> sessionRepo)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _hubService = hubService;
        _userRepo = userRepo;
        _sessionRepo = sessionRepo;
    }

    public async Task<Result> Handle(
        ForceRevokeUserSessionsCommand request,
        CancellationToken cancellationToken)
    {
        // 1️⃣ حارس بوابة الأمان: التأكد أن الطالب هو الـ Admin، أو نفس المستخدم الذي يطلب طرد أجهزته
        var currentUserId = _currentUserService.UserId;
        var currentUserRole = _currentUserService.RoleType;

        bool isAdmin = currentUserRole != null && currentUserRole.Contains("admin", StringComparison.OrdinalIgnoreCase);
        bool isSelf = currentUserId == request.UserId;

        if (!isAdmin && !isSelf)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var spec = new CurrentSecuritySessionsSpecification(request.UserId);
        var activeSessions = await _sessionRepo
            .GetListAsync(spec, cancellationToken);
        
        if (!activeSessions.Any())
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        
        foreach (var session in activeSessions)
        {
            session.Revoke("Revoked By admin!");

            foreach (var token in session.RefreshTokens)
            {
                token.Revoke();
            }
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _hubService.SendForceLogoutToUserAsync(request.UserId);

        return Result.Success(OperationStatusCode.Success);
    }
}