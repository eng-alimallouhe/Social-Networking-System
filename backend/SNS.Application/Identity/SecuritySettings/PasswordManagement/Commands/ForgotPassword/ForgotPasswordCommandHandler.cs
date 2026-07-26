using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.DTOs;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.PendingUpdates;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Application.Identity.Shared.ValueObjects;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, PasswordResetResponse>
{
    private readonly IRepository<User> _userRepo;
    private readonly IPendingUpdatesService _pendingUpdatesService;
    private readonly ICodeService _codeService;
    private readonly IGeneratorService _generatorService;
    private readonly IUrlGeneratorService _urlGeneratorService;

    public ForgotPasswordCommandHandler(
        IRepository<User> userRepo,
        IPendingUpdatesService pendingUpdatesService,
        ICodeService codeService,
        IGeneratorService generatorService,
        IUrlGeneratorService urlGeneratorService)
    {
        _userRepo = userRepo;
        _pendingUpdatesService = pendingUpdatesService;
        _codeService = codeService;
        _generatorService = generatorService;
        _urlGeneratorService = urlGeneratorService;
    }

    public async Task<Result<PasswordResetResponse>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var identifier = new UserIdentifier(request.Identifier);

        var spec = new UserForLoginSpecification(request.Identifier, identifier.Type);
            
        var user = await _userRepo.GetSingleAsync(spec, cancellationToken);

        if (user == null) return Result<PasswordResetResponse>.Failure(UserStatusCodes.NotFound);

        var token = _generatorService.GenerateSecureString();

        var createPasswordUpdate = new CreatePasswordUpdateDto(user.Id, token);

        var pendingUpdate = await _pendingUpdatesService.CreatePasswordUpdateAsync(createPasswordUpdate, cancellationToken);

#warning another point to delete befor production:
        var savedUpdate = await _pendingUpdatesService.GetPasswordUpdateAsync(createPasswordUpdate.UserId, cancellationToken);

        if (pendingUpdate.IsFailure) return Result<PasswordResetResponse>.Failure(pendingUpdate.StatusCode);

        var redirectUrl = _urlGeneratorService.GeneratePasswordResetUrl(user.Id, token);
        
        var recipientAddress = user.Email;

        if (user.UserSecuritySettings.DefaultCommunicationMethod == CommunicationMethod.RecoveryEmail)
        {
            recipientAddress = user.UserSecuritySettings.RecoveryEmail!;
        }

        var CodeSendDto = new CodeSendRequest(
            UserId: user.Id,
            UserName: user.UserName,
            RecipientAddress: recipientAddress,
            Purpose: SendPurpose.PasswordReset,
            SendMethod: user.UserSecuritySettings.DefaultCommunicationMethod, 
            SendLanguage: user.PreferredLanguage, 
            RedirectUrl: redirectUrl,
            Token: token);

        var codeSendResult = await _codeService.SendCodeAsync(CodeSendDto, cancellationToken);


        if (codeSendResult.IsFailure)
        {
            return Result<PasswordResetResponse>.Failure(codeSendResult.StatusCode);   
        }

        return Result<PasswordResetResponse>.Success(new PasswordResetResponse(
            user.Id,
            token),
            OperationStatusCode.Success);
    }
}
