using SNS.Application.Abstractions.Authentication;
using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.DTOs.Authentication.Verification.Requests;
using SNS.Common.Results;
using SNS.Common.StatusCodes;
using SNS.Common.StatusCodes.Security;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Security;
using SNS.Domain.Security.Entities;
using SNS.Domain.Specifications.Security.PendingUpdates;
using SNS.Domain.Specifications.Security.Users;
using SNS.Domain.Specifications.Security.VerificationCodes;


namespace SNS.Application.Services.Authentication;

public class CodeService : ICodeService
{
    private readonly IRepository<VerificationCode> _codeRepo;
    private readonly IRepository<PendingUpdate> _pendingUpdateRepo;
    private readonly ISoftDeletableRepository<User> _userRepo;
    private readonly IEmailSenderService _emailSender;
    private readonly ISmsSenderService _smsSender;
    private readonly IGeneratorService _generatorService;
    private readonly IUnitOfWork _unitOfWork;

    public CodeService(
        IRepository<VerificationCode> codeRepo,
        IRepository<PendingUpdate> pendingUpdateRepo,
        ISoftDeletableRepository<User> userRepo,
        IEmailSenderService emailSender,
        ISmsSenderService smsSender,
        IGeneratorService generatorService,
        IUnitOfWork unitOfWork)
    {
        _codeRepo = codeRepo;
        _pendingUpdateRepo = pendingUpdateRepo;
        _userRepo = userRepo;
        _emailSender = emailSender;
        _smsSender = smsSender;
        _generatorService = generatorService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> SendCodeAsync(string userIdentifier, CodeType codeType, Guid? pendingUpdateId = null)
    {
        var userId = await ResolveUserIdAsync(userIdentifier);

        if (userId == Guid.Empty)
            return Result.Failure(UserStatusCodes.UserNotFound);

        // Enforce security throttling to prevent spamming
        var throttlingCheck = await CheckThrottlingStatusAsync(userId, codeType);
        if (!throttlingCheck.IsSuccess)
            return throttlingCheck;

        var codeString = _generatorService.GenerateSecureCode();

        var verificationCode = new VerificationCode
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Code = codeString,
            Type = codeType,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            PendingUpdateId = pendingUpdateId
        };

        // Add to memory only. Commit is handled by the calling service (Orchestrator).
        await _codeRepo.AddAsync(verificationCode);

        if (IsEmail(userIdentifier))
            await _emailSender.SendEmailAsync(userIdentifier, "Verification Code", $"Code: {codeString}");
        else
            await _smsSender.SendSmsAsync(userIdentifier, $"Code: {codeString}");

        return Result.Success(VerificationStatusCodes.CodeSent);
    }

    public async Task<Result> VerifyCodeAsync(VerifyCodeDto dto)
    {
        const int MaxAttempts = 3;

        var userId = await ResolveUserIdAsync(dto.UserIdentifier);
        if (userId == Guid.Empty)
            return Result.Failure(UserStatusCodes.UserNotFound);

        // Fetch latest active code. Tracking enabled to update state.
        var activeCode = await _codeRepo.GetSingleAsync(new ActiveVerificationCodeSpecification(userId, dto.CodeType));
        if (activeCode == null)
            return Result.Failure(VerificationStatusCodes.NoActiveCode);

        if (activeCode.ExpiresAt < DateTime.UtcNow)
            return Result.Failure(VerificationStatusCodes.CodeExpired);

        // Pre-validation lockout check
        if (activeCode.FailedAttempts >= MaxAttempts)
        {
            activeCode.IsUsed = true;
            await _unitOfWork.CompleteAsync(); // Persist lockout immediately
            return Result.Failure(VerificationStatusCodes.MaxAttemptsReached);
        }

        if (activeCode.Code != dto.Code)
        {
            activeCode.FailedAttempts++;

            // Persist failure count immediately for security
            await _unitOfWork.CompleteAsync();

            if (activeCode.FailedAttempts >= MaxAttempts)
                return Result.Failure(VerificationStatusCodes.MaxAttemptsReached);

            return Result.Failure(VerificationStatusCodes.InvalidCode);
        }

        // Success
        activeCode.IsUsed = true;
        await _unitOfWork.CompleteAsync();

        return Result.Success(VerificationStatusCodes.CodeVerified);
    }

    public async Task<Result> ResendCodeAsync(string userIdentifier, CodeType codeType)
    {
        return await SendCodeAsync(userIdentifier, codeType);
    }

    // ========================================================================
    // Helpers
    // ========================================================================

    private async Task<Guid> ResolveUserIdAsync(string identifier)
    {
        // Priority: PendingUpdates -> Users
        var pendingSpec = new PendingUpdateByInfoSpecification(identifier);
        var pendingUpdate = await _pendingUpdateRepo.GetSingleAsync(pendingSpec);

        if (pendingUpdate != null)
            return pendingUpdate.UserId;

        var user = await _userRepo.GetSingleAsync(new UserWithCodesAndUpdatesSpecification(identifier));

        return user?.Id ?? Guid.Empty;
    }

    private async Task<Result> CheckThrottlingStatusAsync(Guid userId, CodeType type)
    {
        var historyWindow = DateTime.UtcNow.AddHours(-24);

        var recentCodes = await _codeRepo.GetListByExpressionAsync(
            c => c.UserId == userId &&
                 c.Type == type &&
                 !c.IsUsed &&
                 c.CreatedAt >= historyWindow);

        var sortedCodes = recentCodes.OrderByDescending(c => c.CreatedAt).ToList();

        if (!sortedCodes.Any())
            return Result.Success(VerificationStatusCodes.CodeSent);

        var lastTime = sortedCodes.First().CreatedAt;
        var diff = DateTime.UtcNow - lastTime;

        // Progressive Backoff Policy
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
}