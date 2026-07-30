using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Application.Identity.Users.Registeration.DTOs;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.Registeration.Commands.ResendVerifyCode;

/// <summary>
/// Handles the execution of <see cref="ResendVerifyCodeCommand"/> to issue a new verification code.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Fetches user entity and verifies that the user is active and unverified.
/// 2. Generates a new secure token and constructs the account activation URL.
/// 3. Sends a new verification email code via <see cref="ICodeService"/>.
/// Side effects include code delivery notification generation and dispatching.
/// </remarks>
public sealed class ResendVerifyCodeCommandHandler : ICommandHandler<ResendVerifyCodeCommand, RegisterResponseDto>
{
    private readonly IRepository<User> _userRepo;
    private readonly ICodeService _codeService;
    private readonly IUrlGeneratorService _urlGeneratorService;
    private readonly IGeneratorService _generatorService;
    

    public ResendVerifyCodeCommandHandler(
        IRepository<User> userRepo,
        ICodeService codeService,
        IUrlGeneratorService urlGeneratorService,
        IGeneratorService generatorService)
    {
        _userRepo = userRepo;
        _codeService = codeService;
        _urlGeneratorService = urlGeneratorService;
        _generatorService = generatorService;
    }

    public async Task<Result<RegisterResponseDto>> Handle(ResendVerifyCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null || user.Status != UserStatus.Active)
            return Result<RegisterResponseDto>.Failure(UserStatusCodes.NotFound);

        if (user.IsVerified)
            return Result<RegisterResponseDto>.Failure(UserStatusCodes.Conflict);

        var token = _generatorService.GenerateSecureString();

        var redirectUrl = _urlGeneratorService.GenerateAccountActivationUrl(user.Id, token);
        var sendCodeDto = new CodeSendRequest(
            UserId: user.Id,
            UserName: user.UserName,
            RecipientAddress: user.Email,
            Purpose: SendPurpose.UserVerification,
            SendMethod: CommunicationMethod.Email,
            SendLanguage: user.PreferredLanguage,
            RedirectUrl: redirectUrl,
            Token: token);

        var codeSendResult = await _codeService.SendCodeAsync(sendCodeDto, cancellationToken);

        if (codeSendResult.IsFailure)
        {
            return Result<RegisterResponseDto>.Failure(codeSendResult.StatusCode);
        }

        return Result<RegisterResponseDto>.Success(
            new RegisterResponseDto(UserId: user.Id, Token: token),
            VerificationStatusCodes.CodeSent);
    }
}
