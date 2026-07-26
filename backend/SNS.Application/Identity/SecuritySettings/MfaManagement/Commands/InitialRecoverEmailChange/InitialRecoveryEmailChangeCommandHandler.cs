using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.PendingUpdates;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Domain.Identity.SecuritySettings.Events;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Shared.Events;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.InitialRecoverEmailChange;


public sealed class InitialRecoveryEmailChangeCommandHandler 
    : ICommandHandler<InitialRecoveryEmailChangeCommand, IdentifierChangeResponseDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<User> _userRepo;
    private readonly IPendingUpdatesService _pendingUpdatesService;
    private readonly IGeneratorService _generatorService;
    private readonly ICodeService _codeService;
    private readonly IUrlGeneratorService _urlGenerator;
    private readonly IRequestInfoService _requestInfoService;
    private readonly IUnitOfWork _unitOfWork;

    public InitialRecoveryEmailChangeCommandHandler(
        ICurrentUserService currentUserService,
        IRepository<User> userRepo,
        IPendingUpdatesService pendingUpdatesService,
        IGeneratorService generatorService,
        ICodeService codeService,
        IUrlGeneratorService urlGenerator,
        IRequestInfoService requestInfoService,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _userRepo = userRepo;
        _pendingUpdatesService = pendingUpdatesService;
        _generatorService = generatorService;
        _codeService = codeService;
        _urlGenerator = urlGenerator;
        _requestInfoService = requestInfoService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IdentifierChangeResponseDto>> Handle(InitialRecoveryEmailChangeCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
            
        var sessionId = _currentUserService.SessionId;

        if (userId == null || sessionId == null)
            return Result<IdentifierChangeResponseDto>.Failure(OperationStatusCode.AuthenticationRequired);

        var spec = new UserWithRoleAndSettingsAndProfileSpecification(userId.Value);
        var user = await _userRepo.GetSingleAsync(spec, cancellationToken);

        if (user == null)
            return Result<IdentifierChangeResponseDto>.Failure(UserStatusCodes.NotFound);

        var token = _generatorService.GenerateSecureString();

        var pendingUpdate = new CreateEmailUpdateDto(
            user.Id,
            request.NewEmail,
            token);

        await _pendingUpdatesService.CreateEmailUpdateAsync(pendingUpdate, cancellationToken);

        var redirecrtUrl = _urlGenerator.GenerateSupportUrl(request.NewEmail);

        var codeSendDto = new CodeSendRequest(
            UserId: user.Id,
            UserName: user.UserName,
            RecipientAddress: request.NewEmail,
            Purpose: SendPurpose.RecoveryEmailChangeVerification,
            SendMethod: CommunicationMethod.RecoveryEmail,
            SendLanguage: user.PreferredLanguage,
            RedirectUrl: redirecrtUrl,
            Token: token);

        var alertRecipientAddress = 
            user.UserSecuritySettings.DefaultCommunicationMethod == CommunicationMethod.Email?
            user.Email : user.UserSecuritySettings.RecoveryEmail;


        user.AddDomainEvent(new IdentifierChangeRequestedSynchronousEvent(
            UserId: user.Id,
            UserName: user.UserName,
            Device: _requestInfoService.DeviceName,
            Browser: _requestInfoService.Browser,
            SendLanguage: user.PreferredLanguage,
            IpAddress: _requestInfoService.IpAddress,
            RecipientAddress: alertRecipientAddress!,
            DefaultCommunicationMethod: user.UserSecuritySettings.DefaultCommunicationMethod,
            UpdateType: UpdateType.RecoveryEmail,
            EventType: EventType.Synchronous,
            Longitude: _requestInfoService.Longitude,
            Latitude: _requestInfoService.Latitude,
            Country: _requestInfoService.Country,
            City: _requestInfoService.City,
            OccurredOn: DateTime.UtcNow));

        var sendResult = await _codeService.SendCodeAsync(codeSendDto, cancellationToken);

        if (sendResult.IsFailure)
        {
            return Result<IdentifierChangeResponseDto>.Failure(sendResult.StatusCode);
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        var response = new IdentifierChangeResponseDto(
            UserId: user.Id,
            Token: token,
            CodeExpiryDate: DateTime.UtcNow.AddMinutes(15));

        return Result<IdentifierChangeResponseDto>.Success(response, OperationStatusCode.Success);
    }
}
