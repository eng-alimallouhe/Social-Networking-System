using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySessions.Abstractions;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Events;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.CompleteUserDeactivation;

public sealed class CompleteUserDeactivationCommandHandler : ICommandHandler<CompleteUserDeactivationCommand>
{
    private readonly ICodeService _codeService;
    private readonly IRepository<User> _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserCacheService _userCacheService;
    private readonly IProfileCacheService _profileCacheService;
    private readonly ISessionService _sessionService;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;

    public CompleteUserDeactivationCommandHandler(
        ICodeService codeService,
        IRepository<User> userRepo,
        ISoftDeletableRepository<Profile> profileRepo,
        IUnitOfWork unitOfWork,
        IUserCacheService userCacheService,
        IProfileCacheService profileCacheService,
        ISessionService sessionService)
    {
        _codeService = codeService;
        _userRepo = userRepo;
        _profileRepo = profileRepo;
        _unitOfWork = unitOfWork;
        _userCacheService = userCacheService;
        _profileCacheService = profileCacheService;
        _sessionService = sessionService;
    }

    public async Task<Result> Handle(CompleteUserDeactivationCommand request, CancellationToken cancellationToken)
    {
        var codeVerifyResult = await _codeService.VerifyCodeAsync(new VerifyCodeDto(
            UserId: request.UserId,
            Code: request.Code,
            CodeType: CodeType.UserDeleting,
            Token: request.Token));

        if (codeVerifyResult.IsFailure)
        {
            return Result.Failure(codeVerifyResult.StatusCode);
        }

        var user = await _userRepo.GetByIdAsync(request.UserId, cancellationToken: cancellationToken);

        var profile = await _profileRepo.GetSingleByExpressionAsync(
            p => p.UserId == request.UserId, cancellationToken: cancellationToken);

        if (user == null || profile == null)
        {
            return Result.Failure(UserStatusCodes.NotFound);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

        try
        {
            user.Deactivate();

            profile.SoftDelete();

            user.AddDomainEvent(new UserDeletedEvent(
                UserName: user.UserName,
                Email: user.Email,
                OccurredOn: DateTime.UtcNow));

            await _unitOfWork.CompleteAsync(cancellationToken: cancellationToken);
            
            await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);
            
            await _userCacheService.RemoveUserAsync(user.Id, cancellationToken);
            await _sessionService.ClearSessionsByUserIdAsync(user.Id, cancellationToken);
            await _profileCacheService.RemoveProfileAsync(profile.Id, user.Id, cancellationToken);

            return Result.Success(OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken: cancellationToken);
            throw;
        }
    }
}