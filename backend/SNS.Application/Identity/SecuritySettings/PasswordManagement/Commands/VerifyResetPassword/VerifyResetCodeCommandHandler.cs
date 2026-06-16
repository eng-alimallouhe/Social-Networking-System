using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.DTOs;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.PendingUpdates;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.VerifyResetPassword;

public sealed class VerifyResetCodeCommandHandler : ICommandHandler<VerifyResetCodeCommand, VerifyResetPasswordResponseDto>
{
    private readonly IPendingUpdatesService _pendingUpdatesService;
    private readonly ICodeService _codeService;

    public VerifyResetCodeCommandHandler(
        IPendingUpdatesService pendingUpdatesService,
        ICodeService codeService)
    {
        _codeService = codeService;
        _pendingUpdatesService = pendingUpdatesService;
    }

    public async Task<Result<VerifyResetPasswordResponseDto>> Handle(VerifyResetCodeCommand request, CancellationToken cancellationToken)
    {
        var pendingUpdate = await _pendingUpdatesService.GetPasswordUpdateAsync(request.UserId, cancellationToken);

        if (pendingUpdate == null) 
            return Result<VerifyResetPasswordResponseDto>.Failure(ResourceStatusCode.NotFound);

        if (pendingUpdate.Token != request.Token)
            return Result<VerifyResetPasswordResponseDto>.Failure(OperationStatusCode.AccessDenied);

        var codeVerifyResult = await _codeService.VerifyCodeAsync(new VerifyCodeDto(
            UserId: request.UserId,
            Code: request.Code,
            Token: request.Token,
            CodeType: CodeType.PasswordReset), cancellationToken);

        if (codeVerifyResult.IsFailure)
            return Result<VerifyResetPasswordResponseDto>.Failure(codeVerifyResult.StatusCode);

        var verifiedPasswordUpdate = new VerifiedPasswordUpdateDto(
            request.UserId,
            request.Token);

        await _pendingUpdatesService.ConfirmPasswordUpdateAsync(verifiedPasswordUpdate, cancellationToken);

        return Result<VerifyResetPasswordResponseDto>.Success(new VerifyResetPasswordResponseDto(
            UserId: request.UserId,
            Token: request.Token), OperationStatusCode.Success);
    }
}
