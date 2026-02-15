using Microsoft.Extensions.Options;
using SNS.Application.Abstractions.Authentication;
using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.DTOs.Authentication.Verification.Requests;
using SNS.Application.Settings;
using SNS.Common.Enums;
using SNS.Common.Results;
using SNS.Common.StatusCodes;
using SNS.Common.StatusCodes.Security;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Common.Enums;
using SNS.Domain.Security;
using SNS.Domain.Security.Entities;
using SNS.Domain.Specifications.Security.Users;
using SNS.Domain.Specifications.Security.VerificationCodes;

namespace SNS.Application.Services.Authentication;

/// <summary>
/// Represents the implementation of the verification code service.
/// 
/// This service handles the end-to-end lifecycle of One-Time Passwords (OTPs),
/// including secure generation, throttling (rate limiting), dispatching via 
/// specific channels (Email/SMS), and validation.
/// </summary>
public class CodeService : ICodeService
{
    private readonly IRepository<VerificationCode> _codeRepo;
    private readonly IRepository<PendingUpdate> _pendingUpdateRepo;
    private readonly ISoftDeletableRepository<User> _userRepo;
    private readonly IEmailSenderService _emailSender;
    private readonly ITemplateReaderService _templateReaderService;
    private readonly ISmsSenderService _smsSender;
    private readonly IGeneratorService _generatorService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISmsStructureReader _smsStructureReader;
    private readonly AppSettings _appSettings;

    public CodeService(
        IRepository<VerificationCode> codeRepo,
        IRepository<PendingUpdate> pendingUpdateRepo,
        ISoftDeletableRepository<User> userRepo,
        IEmailSenderService emailSender,
        ITemplateReaderService templateReaderService,
        ISmsSenderService smsSender,
        IGeneratorService generatorService,
        IUnitOfWork unitOfWork,
        IOptions<AppSettings> options,
        ISmsStructureReader smsStructureReader)
    {
        _codeRepo = codeRepo;
        _pendingUpdateRepo = pendingUpdateRepo;
        _userRepo = userRepo;
        _emailSender = emailSender;
        _templateReaderService = templateReaderService;
        _smsSender = smsSender;
        _generatorService = generatorService;
        _unitOfWork = unitOfWork;
        _appSettings = options.Value;
        _smsStructureReader = smsStructureReader;
    }

    // ========================================================================
    // 1. Send Code
    // ========================================================================
    public async Task<Result> SendCodeAsync(
        string userIdentifier,
        Guid userId,
        CodeType codeType,
        SupportedLanguage language,
        Guid? pendingUpdateId = null,
        string? redirectUrl = null)
    {
        // 1. Security: Enforce throttling to prevent SMS flooding or Email spamming.
        var throttlingCheck = await CheckThrottlingStatusAsync(userId, codeType);

        if (!throttlingCheck.IsSuccess)
            return throttlingCheck;

        // 2. Generate new cryptographic code
        var codeString = _generatorService.GenerateSecureCode();

        var verificationCode = new VerificationCode
        {
            UserId = userId,
            Code = codeString,
            Type = codeType,
            PendingUpdateId = pendingUpdateId
        };

        // 3. Security: Revoke previous codes. 
        // We only allow ONE active code per type at a time to prevent "Race Condition" attacks
        // or confusion where a user tries to use an old code that just arrived.
        var oldCodes = await _codeRepo.GetListByExpressionAsync(
            c => c.UserId == userId &&
                 !c.IsUsed &&
                 !c.IsRevoked &&
                 c.Type == codeType);

        foreach (var oldCode in oldCodes)
        {
            oldCode.IsRevoked = true;
        }

        // Add to memory only. Commit is handled by the calling service (Orchestrator).
        await _codeRepo.AddAsync(verificationCode);

        // 4. Dispatch
        // 
        Result messageResult;

        if (IsEmail(userIdentifier))
        {
            messageResult = await SendEmailAsync(
                userIdentifier, codeType, language, codeString, redirectUrl);
        }
        else
        {
            messageResult = await SendSmsAsync(
                userIdentifier, codeType, language, codeString, redirectUrl);
        }

        return Result.Success(messageResult.StatusCode);
    }

    // ========================================================================
    // 2. Verify Code
    // ========================================================================
    public async Task<Result> VerifyCodeAsync(VerifyCodeDto dto)
    {
        const int MaxAttempts = 3;

        var userId = (await ResolveUserIdAsync(dto.UserIdentifier)).Value;

        if (userId == Guid.Empty)
            return Result.Failure(UserStatusCodes.NotFound);

        // Fetch latest active code using the Specification pattern.
        // Tracking is enabled because we need to update FailedAttempts or IsUsed.
        var activeCode = await _codeRepo.GetSingleAsync(
            new ActiveVerificationCodeSpecification(userId, dto.CodeType, dto.PendingUpdateId));

        if (activeCode == null)
            return Result.Failure(VerificationStatusCodes.NoActiveCode);

        if (activeCode.ExpiresAt < DateTime.UtcNow)
            return Result.Failure(VerificationStatusCodes.CodeExpired);

        // Security: Brute Force Protection.
        // If max attempts reached, we implicitly burn the code.
        if (activeCode.FailedAttempts >= MaxAttempts)
        {
            activeCode.IsUsed = true; // Burn it
            return Result.Failure(VerificationStatusCodes.MaxAttemptsReached);
        }

        if (activeCode.Code != dto.Code)
        {
            activeCode.FailedAttempts++;

            if (activeCode.FailedAttempts >= MaxAttempts)
                return Result.Failure(VerificationStatusCodes.MaxAttemptsReached);

            return Result.Failure(VerificationStatusCodes.InvalidCode);
        }

        // Success
        activeCode.IsUsed = true;

        return Result.Success(VerificationStatusCodes.CodeVerified);
    }

    // ========================================================================
    // 3. Resend Code
    // ========================================================================
    public async Task<Result> ResendCodeAsync(
        string userIdentifier,
        CodeType codeType,
        SupportedLanguage language,
        string? redirectUrl = null)
    {
        // We resolve the User ID here because the client might only send the identifier (e.g., email)
        // when requesting a resend.
        var userId = (await ResolveUserIdAsync(userIdentifier)).Value;

        if (userId == Guid.Empty)
            return Result.Failure(UserStatusCodes.NotFound);

        // Reuse the main Send flow which includes Throttling checks
        await SendCodeAsync(
            userIdentifier,
            userId,
            codeType,
            language,
            pendingUpdateId: null, // Resend usually doesn't change pending context
            redirectUrl: redirectUrl);

        // We must commit here because this is often a standalone request
        await _unitOfWork.CompleteAsync();

        return Result.Success(VerificationStatusCodes.CodeResent);
    }

    // ========================================================================
    // Helpers
    // ========================================================================

    private async Task<Result> SendEmailAsync(
        string email, CodeType codeType, SupportedLanguage language, string code, string? redirectUrl)
    {
        var templateReadResult = _templateReaderService.ReadTemplate(
            language,
            DetermineSendPurpose(codeType));

        string template;

        if (templateReadResult.IsFailure)
        {
            // Fallback template if file I/O fails
            template = "Code: {{code}} <br>Click On this Link {{redirectUrl}}";
        }
        else
        {
            template = templateReadResult.Value!;
        }

        template = template.Replace("{{code}}", code);

        if (redirectUrl != null)
        {
            template = template.Replace("{{redirectUrl}}", redirectUrl);
        }

        template = template.Replace("{{logoUrl}}", _appSettings.LogoUrl);

        return await _emailSender.SendEmailAsync(
            email,
            DetermineEmailSubject(codeType, language.ToString()),
            template);
    }

    private async Task<Result> SendSmsAsync(
        string phoneNumber, CodeType codeType, SupportedLanguage language, string code, string? redirectUrl)
    {
        var messageBody = _smsStructureReader.GetSmsBody(
            language,
            DetermineSendPurpose(codeType));

        if (messageBody == null)
        {
            messageBody = language switch
            {
                SupportedLanguage.English => "Hello\n this is your code: \n{{Code}}",
                SupportedLanguage.Arabic => "مرحبا هذا الكود الخاص بك \n{{Code}}",
                _ => "{{Code}}"
            };
        }

        messageBody = messageBody.Replace("{{Code}}", code);

        if (redirectUrl != null)
        {
            messageBody = messageBody.Replace("{{redirectUrl}}", redirectUrl);
        }

        return await _smsSender.SendSmsAsync(phoneNumber, messageBody);
    }

    private async Task<Result<Guid>> ResolveUserIdAsync(string userIdentifier)
    {
        var user = await _userRepo.GetSingleAsync(new UserByIdentifierSpecification(userIdentifier));
        if (user == null)
            return Result<Guid>.Failure(UserStatusCodes.NotFound);

        return Result<Guid>.Success(user.Id, UserStatusCodes.Found);
    }

    /// <summary>
    /// Implements a "Progressive Backoff" throttling policy.
    /// The more codes a user requests in a 24-hour window, the longer they must wait.
    /// </summary>
    private async Task<Result> CheckThrottlingStatusAsync(Guid userId, CodeType type)
    {
        var historyWindow = DateTime.UtcNow.AddHours(-24);

        var recentCodes = await _codeRepo.GetListByExpressionAsync(
            c => c.UserId == userId &&
                 c.Type == type &&
                 !c.IsUsed && // Only count unused/spam attempts
                 c.CreatedAt >= historyWindow);

        var sortedCodes = recentCodes.OrderByDescending(c => c.CreatedAt).ToList();

        if (!sortedCodes.Any())
            return Result.Success(VerificationStatusCodes.CodeSent);

        var lastTime = sortedCodes.First().CreatedAt;
        var diff = DateTime.UtcNow - lastTime;

        // Policy Definition
        (TimeSpan RequiredDelay, StatusCode ErrorCode) policy = sortedCodes.Count switch
        {
            0 => (TimeSpan.Zero, null!),
            1 => (TimeSpan.FromMinutes(10), VerificationStatusCodes.Throttled_Level1),
            2 => (TimeSpan.FromMinutes(30), VerificationStatusCodes.Throttled_Level2),
            3 => (TimeSpan.FromHours(1), VerificationStatusCodes.Throttled_Level3),
            4 => (TimeSpan.FromHours(2), VerificationStatusCodes.Throttled_Level4),
            _ => (TimeSpan.FromHours(12), VerificationStatusCodes.Throttled_LevelMax)
        };

        if (diff < policy.RequiredDelay)
            return Result.Failure(policy.ErrorCode);

        return Result.Success(VerificationStatusCodes.CodeSent);
    }

    private bool IsEmail(string input) => input.Contains("@");

    private SendPurpose DetermineSendPurpose(CodeType codeType) => codeType switch
    {
        CodeType.AccountActivation => SendPurpose.AccountActivation,
        CodeType.PasswordReset => SendPurpose.PasswordReset,
        CodeType.LoginTwoFactor => SendPurpose.LoginTwoFactor,
        CodeType.ChangeEmail => SendPurpose.EmailChangeVerification,
        CodeType.ChangePhoneNumber => SendPurpose.PhoneChangeVerification,
        CodeType.SupportChangePhoneNumber => SendPurpose.SupportPhoneChangeRequest,
        _ => SendPurpose.AccountActivation
    };

    private string DetermineEmailSubject(CodeType codeType, string languageCode)
    {
        var lang = languageCode?.ToLower().Trim().Substring(0, 2) ?? "en";

        return (codeType, lang) switch
        {
            (CodeType.AccountActivation, "ar") => "تفعيل حسابك الجديد",
            (CodeType.PasswordReset, "ar") => "رمز استعادة كلمة المرور",
            (CodeType.LoginTwoFactor, "ar") => "رمز التحقق للدخول",
            (CodeType.ChangeEmail, "ar") => "تأكيد تغيير البريد الإلكتروني",
            (CodeType.ChangePhoneNumber, "ar") => "تأكيد تغيير رقم الهاتف",
            (CodeType.SupportChangePhoneNumber, "ar") => "تنبيه: طلب تغيير رقم الهاتف من الدعم الفني",

            (CodeType.AccountActivation, _) => "Activate Your Account",
            (CodeType.PasswordReset, _) => "Password Reset Verification Code",
            (CodeType.LoginTwoFactor, _) => "Two-Factor Authentication Code",
            (CodeType.ChangeEmail, _) => "Verify Email Change Request",
            (CodeType.ChangePhoneNumber, _) => "Verify Phone Number Change",
            (CodeType.SupportChangePhoneNumber, _) => "Action Required: Support Phone Change Request",

            _ => lang == "ar" ? "رمز التحقق" : "Verification Code"
        };
    }
}