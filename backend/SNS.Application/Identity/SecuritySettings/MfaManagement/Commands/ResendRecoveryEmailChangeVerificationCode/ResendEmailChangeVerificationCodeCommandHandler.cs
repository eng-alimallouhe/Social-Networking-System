using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.PendingUpdates;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Domain.Identity.SecuritySettings.Events;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Shared.Events;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.EmailChange.Commands.ResendRecoveryEmailChangeVerificationCode;

public sealed class ResendRecoveryEmailChangeVerificationCodeCommandHandler : 
    ICommandHandler<ResendRecoveryEmailChangeVerificationCodeCommand, IdentifierChangeResponseDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ICodeService _codeService;
    private readonly IRepository<User> _userRepo;
    private readonly IUrlGeneratorService _urlGenerator;
    private readonly IPendingUpdatesService _pendingUpdatesService;
    private readonly IGeneratorService _generatorService;
    private readonly IRequestInfoService _requestInfoService;

    public ResendRecoveryEmailChangeVerificationCodeCommandHandler(
        ICurrentUserService currentUserService,
        ICodeService codeService,
        IRepository<User> userRepo,
        IUrlGeneratorService urlGenerator,
        IPendingUpdatesService pendingUpdatesService,
        IGeneratorService generatorService,
        IRequestInfoService requestInfoService)
    {
        _currentUserService = currentUserService;
        _codeService = codeService;
        _userRepo = userRepo;
        _urlGenerator = urlGenerator;
        _pendingUpdatesService = pendingUpdatesService;
        _generatorService = generatorService;
        _requestInfoService = requestInfoService;
    }

    public async Task<Result<IdentifierChangeResponseDto>> Handle(ResendRecoveryEmailChangeVerificationCodeCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var sessionId = _currentUserService.SessionId;

        if (userId == null || sessionId == null)
        {
            return Result<IdentifierChangeResponseDto>.Failure(OperationStatusCode.AuthenticationRequired);
        }

        var user = await _userRepo.GetByIdAsync(userId.Value, cancellationToken);

        if (user == null)
            return Result<IdentifierChangeResponseDto>.Failure(UserStatusCodes.NotFound);
         
        var emailUpdate = await _pendingUpdatesService.GetEmailUpdateAsync(userId.Value, cancellationToken);

        if (emailUpdate == null || emailUpdate.Token != request.Token)
        {
            return Result<IdentifierChangeResponseDto>.Failure(ResourceStatusCode.NotFound);
        }

        var token = _generatorService.GenerateSecureCode();
        
        var redirectUrl = _urlGenerator.GenerateEmailChangeVerificationUrl(emailUpdate.NewEmail, emailUpdate.Token);

        var codeSendRequestDto = new CodeSendRequest(
            UserId: user.Id,
            UserName: user.UserName,
            RecipientAddress: emailUpdate.NewEmail,
            Purpose: SendPurpose.RecoveryEmailChangeVerification,
            SendMethod: CommunicationMethod.RecoveryEmail,
            SendLanguage: user.PreferredLanguage,
            RedirectUrl: redirectUrl,
            Token: token);

        var alertRecipientAddress =
            user.UserSecuritySettings.DefaultCommunicationMethod == CommunicationMethod.Email ?
            user.Email : user.UserSecuritySettings.RecoveryEmail;

        var newUpdate = new CreateEmailUpdateDto(
            UserId: user.Id,
            NewEmail: emailUpdate.NewEmail,
            Token: emailUpdate.Token);

        await _pendingUpdatesService.CreateEmailUpdateAsync(newUpdate, cancellationToken);

        user.AddDomainEvent(new IdentifierChangeRequestedSynchronousEvent(
            UserId: user.Id,
            UserName: user.UserName,
            Device: _requestInfoService.DeviceName,
            Browser: _requestInfoService.Browser,
            SendLanguage: user.PreferredLanguage,
            IpAddress: _requestInfoService.IpAddress,
            RecipientAddress: alertRecipientAddress!,
            DefaultCommunicationMethod: user.UserSecuritySettings.DefaultCommunicationMethod,
            UpdateType: UpdateType.Email,
            City: _requestInfoService.City,
            Country: _requestInfoService.Country,
            Longitude: _requestInfoService.Longitude,
            Latitude: _requestInfoService.Latitude,
            EventType: EventType.Synchronous,
            OccurredOn: DateTime.UtcNow));


        var sendResult = await _codeService.SendCodeAsync(codeSendRequestDto, cancellationToken);

        if (sendResult.IsFailure)
            return Result<IdentifierChangeResponseDto>.Failure(sendResult.StatusCode);


        return Result<IdentifierChangeResponseDto>.Success(
            new IdentifierChangeResponseDto(
                UserId: user.Id,
                Token: token, 
                CodeExpiryDate: DateTime.UtcNow.AddMinutes(15)), OperationStatusCode.Success);
    }
}
