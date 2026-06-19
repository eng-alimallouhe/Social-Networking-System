using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Archives;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Domain.Identity.ArchiveManagement.Enums;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.VerifyRecoveryEmailChange;

public sealed class VerifyRecoveryEmailChangeCommandHandler : ICommandHandler<VerifyRecoveryEmailChangeCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPendingUpdatesService _pendingUpdateService;
    private readonly IRequestInfoService _requestInfoService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<User> _userRepo;
    private readonly ICodeService _codeService;
    private readonly IArchiveService _archiveService;

    public VerifyRecoveryEmailChangeCommandHandler(
        IUnitOfWork unitOfWork,
        IPendingUpdatesService pendingUpdatesService,
        IRequestInfoService requestInfoService,
        ICurrentUserService currentUserService,
        IRepository<User> userRepo,
        ICodeService codeService,
        IArchiveService archiveService)
    {
        _unitOfWork = unitOfWork;
        _pendingUpdateService = pendingUpdatesService;
        _requestInfoService = requestInfoService;
        _currentUserService = currentUserService;
        _userRepo = userRepo;
        _codeService = codeService;
        _archiveService = archiveService;
    }

    public async Task<Result> Handle(VerifyRecoveryEmailChangeCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
            return Result<AuthTokensDto>.Failure(OperationStatusCode.AuthenticationRequired);

        var spec = new UserWithRoleAndSettingsSpecification(userId.Value);

        var user = await _userRepo.GetSingleAsync(spec, cancellationToken);

        if (user == null)
            return Result<AuthTokensDto>.Failure(UserStatusCodes.NotFound);

        var pendingUpdate = await _pendingUpdateService.GetEmailUpdateAsync(user.Id, cancellationToken);

        if (pendingUpdate == null)
            return Result<AuthTokensDto>.Failure(OperationStatusCode.ResourceNotFound);

        var verifyCodeDto = new VerifyCodeDto(
            UserId: userId.Value,
            Token: request.Token,
            Code: request.Code, 
            CodeType: CodeType.ChangeRecoveryEmail);
        
        var codeVerifyResult = await _codeService.VerifyCodeAsync(verifyCodeDto, cancellationToken);

        if (codeVerifyResult.IsFailure)
            return Result.Failure(codeVerifyResult.StatusCode);


        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var oldIdentifier = user.UserSecuritySettings.RecoveryEmail ?? "Initial";
            var newIdentifier = pendingUpdate.NewEmail;

            user.UserSecuritySettings.ChangeRecoveryEmail(email: newIdentifier);


            await _archiveService.ArchiveIdentityAsync(
                new CreateIdentityArchiveDto(UserId: user.Id, OldIdentifier: oldIdentifier, NewIdentifier: newIdentifier, IdentityType: IdentityType.RecoveryEmail), cancellationToken);

            await _archiveService.LogUserActionAsync(
                new CreateUserArchiveDto(
                    UserId: user.Id, 
                    ActionType: ActionType.EmailChanged, 
                    PerformedBy: user.Id,
                    Parameters: new Dictionary<ReplacementKey, string>
                    {
                        { ReplacementKey.Device, _requestInfoService.DeviceName },
                        { ReplacementKey.NewEmail, newIdentifier }
                    }), cancellationToken);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await _pendingUpdateService.DeleteEmailUpdateAsync(user.Id);


            return Result.Success(OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
