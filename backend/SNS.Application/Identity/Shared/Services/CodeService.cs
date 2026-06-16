using Microsoft.Extensions.Options;
using SNS.Application.Abstractions.Caching;
using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Application.Shared.Contracts.Messaging;
using SNS.Application.Shared.Settings;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Shared.Services;

/// <summary>
/// Represents the implementation of the verification code service.
/// </summary>
public class CodeService : ICodeService
{
    private readonly IIdentityCacheKeyFactory _identityCacheKeyFactory;
    private readonly ICacheService _cacheService;
    private readonly IEmailSenderService _emailSender;
    private readonly IEmailTemplateProvider _templateProvider;
    private readonly IGeneratorService _generatorService;
    private readonly AppSettings _appSettings;
    

    private readonly TimeSpan _codeExpiry = TimeSpan.FromMinutes(10);
    private readonly TimeSpan _cooldownPeriod = TimeSpan.FromSeconds(60);
    private readonly TimeSpan _attemptsExpiry = TimeSpan.FromHours(24);

    public CodeService(
        IIdentityCacheKeyFactory identityCacheKeyFactory,
        ICacheService cacheService,
        IEmailSenderService emailSender,
        IEmailTemplateProvider templateProvider,
        IGeneratorService generatorService,
        IOptions<AppSettings> options)
    {
        _identityCacheKeyFactory = identityCacheKeyFactory;
        _cacheService = cacheService;
        _emailSender = emailSender;
        _templateProvider = templateProvider;
        _generatorService = generatorService;
        _appSettings = options.Value;
    }

    public async Task<Result> SendCodeAsync(
        CodeSendRequest dto,
        CancellationToken cancellationToken = default)
    {
        var cooldownKey = _identityCacheKeyFactory.GetCoolDownKey(dto.UserId);
        
        var otpKey = _identityCacheKeyFactory.GetOtpKey(dto.UserId);
        
        var attemptsKey = _identityCacheKeyFactory.GetAttemptsKey(dto.UserId);


        var isCoolingDownTask = _cacheService.ExistsAsync(cooldownKey);
        
        var currentAttemptsTask = _cacheService.GetAsync<int>(attemptsKey);

        await Task.WhenAll(isCoolingDownTask, currentAttemptsTask);

        var isCoolingDown = isCoolingDownTask.Result;

        var currentAttempts = currentAttemptsTask.Result;

        if (isCoolingDown)
            return Result.Failure(VerificationStatusCodes.Throttled);

        if (currentAttempts >= 8)
            return Result.Failure(VerificationStatusCodes.MaxAttemptsReached);

        var codeString = _generatorService.GenerateSecureCode();

        var codeModel = new VerificationCodeModel()
        {
            Code = codeString,
            CurrentAttempt = 0,
            ExipresAt = DateTime.UtcNow.Add(_codeExpiry),
        };

        currentAttempts = (int) await _cacheService.IncrementAsync(attemptsKey, cancellationToken);

        if (currentAttempts == 1)
        {
            await _cacheService.SetKeyExpiryAsync(attemptsKey, _attemptsExpiry, cancellationToken);
        }

        await _cacheService.SetAsync(otpKey, codeModel, _codeExpiry);

        await _cacheService.SetAsync(cooldownKey, true, _cooldownPeriod);

        var replacements = new List<MessageReplacement>
        {
            new MessageReplacement(ReplacementKey.UserName, dto.UserName),
            new MessageReplacement (ReplacementKey.Code, codeString),
            new MessageReplacement (ReplacementKey.RedirectUrl, dto.RedirectUrl),
            new MessageReplacement (ReplacementKey.LogoUrl, _appSettings.LogoUrl)
        };

        var emailTemplate = await _templateProvider.ReadTemplate(dto.SendLanguage, dto.Purpose, replacements);

        await _emailSender.SendEmailAsync(dto.RecipientAddress, emailTemplate.Subject, emailTemplate.Body, cancellationToken);

        Console.WriteLine("-------------------------------------------------------------------------------------");
        Console.WriteLine("-------------------------------------------------------------------------------------");
        Console.WriteLine($"============ Code:{{ {codeModel.Code} }}============================================");
        Console.WriteLine("-------------------------------------------------------------------------------------");
        Console.WriteLine("-------------------------------------------------------------------------------------");

        return Result.Success(VerificationStatusCodes.CodeSent);
    }

    public async Task<Result> VerifyCodeAsync(VerifyCodeDto dto, CancellationToken cancellationToken = default)
    {
        var otpKey = _identityCacheKeyFactory.GetOtpKey(dto.UserId);

        var codeModel = await _cacheService.GetAsync<VerificationCodeModel>(otpKey, cancellationToken);

        if (codeModel == null)
            return Result.Failure(VerificationStatusCodes.NoActiveCode);

        if (codeModel.Token != dto.Token)
            return Result.Failure(OperationStatusCode.TokenInvalid);

        if (codeModel.ExipresAt < DateTime.UtcNow)
        {
            await _cacheService.RemoveAsync(otpKey, cancellationToken);
            return Result.Failure(VerificationStatusCodes.CodeExpired);
        }

        if (codeModel.Code != dto.Code)
        {
            var remainingTTL = codeModel.ExipresAt - DateTime.UtcNow;

            if (remainingTTL <= TimeSpan.Zero)
            {
                await _cacheService.RemoveAsync(otpKey, cancellationToken);
                return Result.Failure(VerificationStatusCodes.NoActiveCode);
            }

            codeModel.CurrentAttempt++;

            if (codeModel.CurrentAttempt >= 5)
            {
                await _cacheService.RemoveAsync(otpKey, cancellationToken);
                return Result.Failure(VerificationStatusCodes.MaxAttemptsReached);
            }

            await _cacheService.SetAsync(otpKey, codeModel, remainingTTL, cancellationToken);

            return Result.Failure(VerificationStatusCodes.InvalidCode);
        }

        await _cacheService.RemoveAsync(otpKey, cancellationToken);

        return Result.Success(VerificationStatusCodes.CodeVerified);
    }
}
