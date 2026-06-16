using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySessions.Abstractions;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Identity.SecuritySessions.Commands.Logout;

public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<SecuritySession> _sessionRepo;
    private readonly ISessionService _sessionService;
    private readonly IUserCacheService _userCacheService;


    public LogoutCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IRepository<SecuritySession> sessionRepo,
        IUserCacheService userCacheService,
        ISessionService sessionService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _sessionRepo = sessionRepo;
        _userCacheService = userCacheService;
        _sessionService = sessionService;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var currentSessionId = _currentUserService.SessionId;

        var currentUserId = _currentUserService.UserId;

        if (!currentSessionId.HasValue || !currentUserId.HasValue)
        {
            return Result.Failure(OperationStatusCode.AuthenticationRequired);
        }

        
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _sessionService.KillSwitchSessionAsync(currentSessionId.Value, cancellationToken);
            
            await _userCacheService.RemoveUserAsync(currentUserId.Value, cancellationToken);

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
