using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Domain.Identity.SecuritySettings.Enums;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySessions.Login.Commands.ResendTwoFactorCode;

public sealed class ResendTwoFactorCodeCommandHandler : ICommandHandler<ResendTwoFactorCodeCommand>
{
    private readonly IRepository<User> _userRepo;
    private readonly IUrlGeneratorService _urlGenerator;
    private readonly ICodeService _codeService;
    private readonly IGeneratorService _generatorService;

    public ResendTwoFactorCodeCommandHandler(
        IRepository<User> userRepo,
        IUrlGeneratorService urlGenerator,
        ICodeService codeService,
        IGeneratorService generatorService)
    {
        _userRepo = userRepo;
        _urlGenerator = urlGenerator;
        _codeService = codeService;
        _generatorService = generatorService;
    }

    public async Task<Result> Handle(ResendTwoFactorCodeCommand request, CancellationToken cancellationToken)
    {
        var spec = new UserWithSecuritySettingsSpecification(request.UserId);
        var user = await _userRepo.GetSingleAsync(spec, cancellationToken);

        if (user == null)
        {
            return Result.Failure(UserStatusCodes.NotFound);
        }

        if (!user.UserSecuritySettings.IsMfaEnabled 
            && user.UserSecuritySettings.MfaProvider == MfaProvider.None
            && user.UserSecuritySettings.MfaProvider == MfaProvider.AuthenticatorApp
            && user.UserSecuritySettings.MfaProvider == MfaProvider.Passkey)
        {
            return Result.Failure(OperationStatusCode.Conflict);
        }


        var token = _generatorService.GenerateSecureString();
        
        var redirectUrl = _urlGenerator.GenerateTFARedirectUrl(user.Id, token);

        var sendMethod = request.ResendMethod ?? user.UserSecuritySettings.DefaultCommunicationMethod;

        string recipientAddress = sendMethod == CommunicationMethod.RecoveryEmail
            ? user.UserSecuritySettings.RecoveryEmail!
            : user.Email;

        var codeSendDto = new CodeSendRequest(
            UserId: user.Id,
            UserName: user.UserName,
            RecipientAddress: recipientAddress,
            Purpose: SendPurpose.LoginTwoFactor,
            SendMethod: sendMethod,
            SendLanguage: user.PreferredLanguage,
            RedirectUrl: redirectUrl,
            Token: token);

        var sendResult = await _codeService.SendCodeAsync(codeSendDto, cancellationToken);

        if (sendResult.IsFailure)
        {
            return Result.Failure(sendResult.StatusCode);
        }

        return Result.Success(OperationStatusCode.Success);
    }
}