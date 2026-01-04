using SNS.Application.Abstractions.Authentication;
using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Abstractions.Security;
using SNS.Common.Results;
using SNS.Common.StatusCodes.Common;
using SNS.Common.StatusCodes.Security;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Common.Enums;
using SNS.Domain.Security;
using SNS.Domain.Security.Entities;
using SNS.Domain.Security.Enums;
using SNS.Domain.Specifications.Security.Users;

namespace SNS.Application.Services.Authentication;

/// <summary>
/// Represents the implementation of the registration service.
/// 
/// This service handles the onboarding of new users, including:
/// 1. Initial Registration (Creation of inactive accounts).
/// 2. Account Activation (Verification of phone/email).
/// 3. Re-sending activation requests.
/// </summary>
public class RegisterService : IRegisterService
{
    private readonly ISoftDeletableRepository<User> _userRepo;
    private readonly ISoftDeletableRepository<Role> _roleRepo;
    private readonly IPendingUpdatesService _pendingUpdateService;
    private readonly ICodeService _codeService;
    private readonly IUserSessionService _sessionService;
    private readonly ITokenService _tokenService;
    private readonly IArchiveService _archiveService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGeneratorService _generatorService;
    private readonly IHashingService _hashingService;
    private readonly IAppLogger<RegisterService> _logger;
    private readonly ICurrentUserService _currentUserService;

    public RegisterService(
        ISoftDeletableRepository<User> userRepo,
        ISoftDeletableRepository<Role> roleRepo,
        IPendingUpdatesService pendingUpdateService,
        ICodeService codeService,
        IUserSessionService sessionService,
        ITokenService tokenService,
        IHashingService hashingService,
        IArchiveService archiveService,
        IUnitOfWork unitOfWork,
        IGeneratorService generatorService,
        IAppLogger<RegisterService> logger,
        ICurrentUserService currentUserService)
    {
        _userRepo = userRepo;
        _roleRepo = roleRepo;
        _pendingUpdateService = pendingUpdateService;
        _codeService = codeService;
        _sessionService = sessionService;
        _tokenService = tokenService;
        _archiveService = archiveService;
        _unitOfWork = unitOfWork;
        _generatorService = generatorService;
        _hashingService = hashingService;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    // ========================================================================
    // 1. Register Process (Start)
    // ========================================================================



    public async Task<Result<RegisterResponseDto>> RegisterAsync(RegisterDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // 1. Check for Existing Users
            // We check both active and inactive users to prevent duplicates or hijacking.
            var existingUser = await _userRepo.GetSingleAsync(
                new UserWithCodesAndUpdatesSpecification(dto.PhoneNumber));

            if (existingUser != null)
            {
                if (existingUser.IsActive)
                {
                    // Active user already owns this phone number.
                    return Result<RegisterResponseDto>.Failure(UserStatusCodes.AlreadyExists);
                }
                else if (existingUser.PendingUpdates.Any(p => p.UpdatedInfo == dto.PhoneNumber))
                {
                    // User exists but is not verified (stuck in onboarding).
                    return Result<RegisterResponseDto>.Failure(UserStatusCodes.NotVerified);
                }
                else
                {
                    // Fallback for inconsistent state
                    return Result<RegisterResponseDto>.Failure(UserStatusCodes.AlreadyExists);
                }
            }

            // 2. Prepare User Data
            var username = await GenerateUniqueUsernameAsync();

            var usersRole = await _roleRepo.GetSingleByExpressionAsync(r => r.Type == RoleType.User);
            if (usersRole == null) return Result<RegisterResponseDto>.Failure(OperationStatusCode.Failure);

            var securityCode = _generatorService.GenerateSecureString();

            // Resolve Language
            var langFromRequest = _currentUserService.Language;
            var language = langFromRequest.StartsWith("ar", StringComparison.OrdinalIgnoreCase)
                ? SupportedLanguage.Arabic
                : SupportedLanguage.English;

            var newUser = new User()
            {
                RoleId = usersRole.Id,
                UserName = username,
                PasswordHash = _hashingService.Hash(dto.Password),
                SecurityCode = securityCode,
                PreferredLanguage = language,
                IsActive = false // Explicitly inactive until verified
            };

            // 3. Create Pending Update (Staging Area)
            // We store the phone number in a PendingUpdate record rather than on the User directly
            // until it is verified.
            var pendingResult = await _pendingUpdateService.CreateOrReplaceAsync(
                newUser.Id,
                UpdateType.Register,
                dto.PhoneNumber);

            if (pendingResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<RegisterResponseDto>.Failure(pendingResult.StatusCode);
            }

            // 4. Persist
            await _userRepo.AddAsync(newUser);

            var pendingUpdateId = pendingResult.Value;

            // 5. Send Verification Code
            var sendResult = await _codeService.SendCodeAsync(
                dto.PhoneNumber,
                newUser.Id,
                CodeType.AccountActivation,
                language,
                pendingUpdateId
            );

            if (sendResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<RegisterResponseDto>.Failure(sendResult.StatusCode);
            }

            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();

            return Result<RegisterResponseDto>.Success(new RegisterResponseDto
            {
                UserId = newUser.Id,
                SecurityCode = securityCode
            }, VerificationStatusCodes.CodeSent);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError("Error while registering a new user.", ex, dto.PhoneNumber);
            return Result<RegisterResponseDto>.Failure(OperationStatusCode.Failure);
        }
    }


    // ========================================================================
    // 2. Activation Process (Finalize)
    // ========================================================================



    public async Task<Result<AuthTokensDto>> ActiveAccountAsync(AccountActivationDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. Verify the Code
            var verifyResult = await _codeService.VerifyCodeAsync(new VerifyCodeDto
            {
                UserIdentifier = dto.PhoneNumber,
                Code = dto.Code,
                CodeType = CodeType.AccountActivation
            });

            if (!verifyResult.IsSuccess) return Result<AuthTokensDto>.Failure(verifyResult.StatusCode);

            // 2. Retrieve Context (Pending Update)
            var pendingUpdateResult = await _pendingUpdateService.GetPendingByInfoAsync(dto.PhoneNumber, UpdateType.Register);

            if (!pendingUpdateResult.IsSuccess || pendingUpdateResult.Value == null)
            {
                return Result<AuthTokensDto>.Failure(UserStatusCodes.NotFound);
            }

            var pendingUpdate = pendingUpdateResult.Value;
            var userId = pendingUpdate.UserId;

            // 3. Retrieve User
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return Result<AuthTokensDto>.Failure(UserStatusCodes.NotFound);

            // 4. Activate User & Commit Phone
            user.IsActive = true;
            user.PhoneNumber = pendingUpdate.UpdatedInfo;

            // Clean up the staging record
            await _pendingUpdateService.DeleteAsync(pendingUpdate.Id);

            // 5. Establish Session
            var sessionResult = await _sessionService.CreateSessionAsync(userId);
            if (!sessionResult.IsSuccess) return Result<AuthTokensDto>.Failure(sessionResult.StatusCode);

            var sessionId = sessionResult.Value;

            // 6. Generate Tokens
            var refreshTokenString = await _tokenService.GrantRefreshTokenAsync(user);
            var accessTokenString = _tokenService.GenerateAccessToken(user, sessionId);

            // 7. Archiving & Logging
            await _archiveService.ArchiveIdentityAsync(userId, user.PhoneNumber, IdentityType.PhoneNumber);
            await _archiveService.ArchivePasswordAsync(userId, user.PasswordHash);
            await _archiveService.LogUserActionAsync(userId, ActionType.AccountActivated, userId, "Account activated via SMS.");

            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();

            return Result<AuthTokensDto>.Success(
                new AuthTokensDto
                {
                    Token = accessTokenString,
                    RefreshToken = refreshTokenString
                },
                UserStatusCodes.Created);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError($"Error while activating the account for phone number {dto.PhoneNumber}", ex, dto.PhoneNumber);
            return Result<AuthTokensDto>.Failure(OperationStatusCode.Failure);
        }
    }


    // ========================================================================
    // 3. Resend Request
    // ========================================================================
    public async Task<Result<RegisterResponseDto>> ResendActiveRequestAsync(ResendActiveRequestDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // 1. Validate User State
            var user = await _userRepo.GetSingleAsync(new UserWithCodesAndUpdatesSpecification(dto.PhoneNumber));

            if (user == null || user.IsActive)
            {
                // Can't resend activation for a user that doesn't exist or is already active.
                return Result<RegisterResponseDto>.Failure(UserStatusCodes.NotFound);
            }

            // 2. Validate Pending Context
            var pendingUpdateResult = await _pendingUpdateService.GetPendingByInfoAsync(dto.PhoneNumber, UpdateType.Register);
            if (!pendingUpdateResult.IsSuccess || pendingUpdateResult.Value == null)
            {
                return Result<RegisterResponseDto>.Failure(UserStatusCodes.NotFound);
            }

            var pendingUpdate = pendingUpdateResult.Value;

            // 3. Resend Code
            var sendResult = await _codeService.SendCodeAsync(
                dto.PhoneNumber,
                user.Id,
                CodeType.AccountActivation,
                user.PreferredLanguage,
                pendingUpdate.Id
            );

            if (sendResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<RegisterResponseDto>.Failure(sendResult.StatusCode);
            }

            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();

            return Result<RegisterResponseDto>.Success(
                new RegisterResponseDto
                {
                    UserId = user.Id,
                    SecurityCode = null // Not needed for resend
                },
                VerificationStatusCodes.CodeSent);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError($"Error while resend the activation code", ex, dto.PhoneNumber);
            return Result<RegisterResponseDto>.Failure(OperationStatusCode.Failure);
        }
    }

    // ========================================================================
    // Helpers
    // ========================================================================

    private async Task<string> GenerateUniqueUsernameAsync()
    {
        string[] allowedNames = { "member", "profile", "persona", "connect", "nova", "spark", "atlas", "node", "circle", "echo", "pulse", "horizon" };

        var randomName = allowedNames[Random.Shared.Next(allowedNames.Length)];
        var randomSuffix = _generatorService.GenerateSecureCode().Substring(0, 4);
        var username = $"{randomName}{randomSuffix}";

        // Simple retry loop to ensure uniqueness
        while (await _userRepo.CountAsync(u => u.UserName == username) > 0)
        {
            randomSuffix = _generatorService.GenerateSecureCode().Substring(0, 4);
            username = $"{randomName}{randomSuffix}";
        }

        return username;
    }
}