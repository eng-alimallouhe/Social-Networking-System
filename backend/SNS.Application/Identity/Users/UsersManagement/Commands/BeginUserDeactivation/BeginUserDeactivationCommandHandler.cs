using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.BeginUserDeactivation;

/// <summary>
/// Handles the execution of <see cref="BeginUserDeactivationCommand"/> to initiate account deactivation.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user identity and fetches user entity with security settings.
/// 2. Generates a secure token and URL for account deactivation.
/// 3. Sends a verification code to the user's preferred communication method.
/// 4. Updates the user's purge preference for hard deletion and persists changes.
/// Side effects include code delivery notification dispatching and entity setting update.
/// </remarks>
public sealed class BeginUserDeactivationCommandHandler :
    ICommandHandler<BeginUserDeactivationCommand, BeginUserDeactivationResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICodeService _codeService;
    private readonly IRepository<User> _userRepo;
    private readonly IUrlGeneratorService _urlGeneratorService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IGeneratorService _generatorService;

    public BeginUserDeactivationCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<User> userRepo,
        ICodeService codeService,
        IGeneratorService generatorService,
        IUrlGeneratorService urlGeneratorService,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _generatorService = generatorService;
        _userRepo = userRepo;
        _codeService = codeService;
        _urlGeneratorService = urlGeneratorService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BeginUserDeactivationResponse>> Handle(BeginUserDeactivationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return Result<BeginUserDeactivationResponse>.Failure(OperationStatusCode.AuthenticationRequired);
        }

        var spec  = new UserWithRoleAndSettingsAndProfileSpecification(userId.Value);

        var user = await _userRepo.GetSingleAsync(spec, cancellationToken);
            
        if (user == null)
        {
            return Result<BeginUserDeactivationResponse>.Failure(UserStatusCodes.NotFound);
        }

        var token = _generatorService.GenerateSecureString();

        var redirectUrl = _urlGeneratorService.GenerateUserDeletingUrl(userId: user.Id, token: token);

        var recipientAddress = user.UserSecuritySettings.DefaultCommunicationMethod switch
        {
            CommunicationMethod.RecoveryEmail => user.UserSecuritySettings.RecoveryEmail!,
            CommunicationMethod.Email => user.Email,
            _ => user.Email
        };

        var codeSendDto = new CodeSendRequest(
            UserId: user.Id,
            UserName: user.UserName,
            RecipientAddress: recipientAddress,
            Purpose: SendPurpose.UserDeleting,
            SendMethod: user.UserSecuritySettings.DefaultCommunicationMethod,
            SendLanguage: user.PreferredLanguage,
            RedirectUrl: redirectUrl,
            Token: token);

        var sendResult = await _codeService.SendCodeAsync(codeSendDto, cancellationToken);

        if (sendResult.IsFailure)
        {
            return Result<BeginUserDeactivationResponse>.Failure(sendResult.StatusCode);
        }

        user.SetPurgeAllContentOnHardDelete();

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<BeginUserDeactivationResponse>.Success(
            new BeginUserDeactivationResponse(
                UserId: user.Id,
                Token: token), sendResult.StatusCode);
    }
}
