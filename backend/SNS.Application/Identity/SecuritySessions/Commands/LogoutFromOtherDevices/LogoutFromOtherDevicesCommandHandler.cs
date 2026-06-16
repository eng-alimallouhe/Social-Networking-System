using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Identity.SecuritySessions.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySessions.Commands.LogoutFromOtherDevices;

public sealed class LogoutFromOtherDevicesCommandHandler : ICommandHandler<LogoutFromOtherDevicesCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<SecuritySession> _sessionRepo;


    public LogoutFromOtherDevicesCommandHandler(
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IRepository<SecuritySession> sessionRepo)
    {
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _sessionRepo = sessionRepo;
    }

    public async Task<Result> Handle(LogoutFromOtherDevicesCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var currentSessionId = _currentUserService.SessionId;

        if (userId == null || currentSessionId == null)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var spec = new OtherUserSessionsSpecification(userId: userId.Value, currentSessionId: currentSessionId.Value);

        var sessions = await _sessionRepo.GetListAsync(spec, cancellationToken);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var session in sessions)
            {
                session.Logout(DateTime.UtcNow);

                foreach (var token in session.RefreshTokens)
                {
                    token.Revoke();
                }

            }
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