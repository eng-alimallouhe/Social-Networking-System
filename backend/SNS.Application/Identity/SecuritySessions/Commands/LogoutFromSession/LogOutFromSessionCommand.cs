using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Identity.SecuritySessions.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySessions.Commands.LogoutFromSession;

public sealed record LogOutFromSessionCommand(Guid SessionId) : ICommand;

public sealed class LogOutFromSessionCommandHandler : ICommandHandler<LogOutFromSessionCommand>
{
    private readonly IRepository<SecuritySession> _sessionRepo; // الكتابة والتتبع عبر الـ Repo الحصين 🏗️
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public LogOutFromSessionCommandHandler(
        IRepository<SecuritySession> sessionRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _sessionRepo = sessionRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(LogOutFromSessionCommand request, CancellationToken cancellationToken)
    {
        // 1️⃣ حارس بوابة الهوية للمستخدم الحالي
        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null || currentUserId == Guid.Empty)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        // 2️⃣ جلب الجلسة من الـ Repository لتدخل تحت التتبع الحركي للـ EF
        var spec = new SecuritySessionWithRefreshTokens(request.SessionId);
        var session = await _sessionRepo.GetSingleAsync(spec, cancellationToken);
        
        if (session == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound); // الجلسة غير موجودة أساساً
        }

        if (session.UserId != currentUserId.Value)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired); 
        }

        if (session.IsRevoked)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        session.Revoke("Revoked By User");

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}