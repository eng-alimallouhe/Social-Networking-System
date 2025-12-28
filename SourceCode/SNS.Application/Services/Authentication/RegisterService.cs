using SNS.Application.Abstractions.Authentication;
using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Security;
using SNS.Application.DTOs.Authentication.Register;
using SNS.Application.DTOs.Authentication.Responses;
using SNS.Common.Results;
using SNS.Common.StatusCodes;
using SNS.Common.StatusCodes.Security;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Security;
using SNS.Domain.Security.Entities;
using SNS.Domain.Security.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Application.Services.Authentication;

public class RegisterService : IRegisterService
{
    private readonly ISoftDeletableRepository<User> _userRepo;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IRepository<PendingUpdate> _pendingRepo;
    private readonly ICodeService _codeService;
    private readonly IPendingUpdatesService _pendingUpdateService;
    private readonly IUserSessionService _sessionService;
    private readonly ITokenService _tokenService;
    private readonly IArchiveService _archiveService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGeneratorService _generatorService;

    public RegisterService(
        ISoftDeletableRepository<User> userRepo,
        ISoftDeletableRepository<Profile> profileRepo,
        IRepository<PendingUpdate> pendingRepo,
        ICodeService codeService,
        IPendingUpdatesService pendingUpdateService,
        IUserSessionService sessionService,
        ITokenService tokenService,
        IArchiveService archiveService,
        IUnitOfWork unitOfWork,
        IGeneratorService generatorService)
    {
        _userRepo = userRepo;
        _profileRepo = profileRepo;
        _pendingRepo = pendingRepo;
        _codeService = codeService;
        _pendingUpdateService = pendingUpdateService;
        _sessionService = sessionService;
        _tokenService = tokenService;
        _archiveService = archiveService;
        _unitOfWork = unitOfWork;
        _generatorService = generatorService;
    }

    // ========================================================================
    // 1. Register Process (Start)
    // ========================================================================
    public async Task<Result<Guid>> RegisterAsync(RegisterDto dto)
    {
        // 1. Check if Phone Number already exists (Active Users)
        var existingUser = await _userRepo.GetSingleByExpressionAsync(u => u.PhoneNumber == dto.PhoneNumber);
        if (existingUser != null)
        {
            // Return specific status: User already exists
            return Result<Guid>.Failure(UserStatusCodes.UserAlreadyExists);
        }

        // 2. Check Pending Updates (Is someone else trying to register this number?)
        var pendingCheck = await _pendingUpdateService.GetPendingByInfoAsync(dto.PhoneNumber, UpdateType.NewRegistration);
        if (pendingCheck.IsSuccess && pendingCheck.Value != null)
        {
            // Scenario: The number is locked in a pending process.
            // Decision: Retrieve the User associated with this pending request to notify the caller.
            // Or simply return a specific status "PendingVerification".
            // Here we return a failure indicating the number is reserved/pending.
            return Result<Guid>.Failure(new StatusCode("Register", 4091)); // 4091: Pending Verification Exists
        }

        // 3. Generate Unique Username
        // Logic: FirstName + LastName + Random 4 digits (handled in helper)
        var username = await GenerateUniqueUsernameAsync(dto.FirstName, dto.LastName);

        // 4. Create User Entity (Inactive)
        var userId = Guid.NewGuid();
        var newUser = new User
        {
            Id = userId,
            UserName = username,
            PhoneNumber = null, // IMPORTANT: Keep null until verified!
            PasswordHash = HashPassword(dto.Password), // Helper method
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            Role = null // Set default role if needed, or handle later
        };

        // 5. Create Empty Profile
        var newProfile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = $"{dto.FirstName} {dto.LastName}",
            Bio = string.Empty
        };

        // 6. Create Pending Update (The staging area for the phone number)
        // Using CreateOrReplace to ensure we don't duplicate requests for the same user ID (though ID is new here)
        var pendingResult = await _pendingUpdateService.CreateOrReplaceAsync(
            userId,
            UpdateType.NewRegistration,
            dto.PhoneNumber);

        var pendingUpdateId = pendingResult.Value;

        // 7. Persist Entities (Transaction Part 1)
        await _userRepo.AddAsync(newUser);
        await _profileRepo.AddAsync(newProfile);
        // PendingUpdate is added inside CreateOrReplaceAsync

        // 8. Send Verification Code (Linked to the Pending Update)
        var sendResult = await _codeService.SendCodeAsync(
            dto.PhoneNumber,
            CodeType.PhoneNumberConfirmation,
            pendingUpdateId); // Linking the code to this specific update context

        if (!sendResult.IsSuccess)
        {
            return Result<Guid>.Failure(sendResult.StatusCode);
        }

        // 9. Commit Transaction
        await _unitOfWork.CompleteAsync();

        return Result<Guid>.Success(userId, VerificationStatusCodes.CodeSent);
    }

    // ========================================================================
    // 2. Activation Process (Finalize)
    // ========================================================================
    public async Task<Result<AuthenticationResultDto>> ActiveAccountAsync(AccountActivationDto dto)
    {
        // 1. Verify the Code
        // Note: CodeService verifies logic (Expiry, Attempts). 
        // We pass the UserIdentifier (Phone) to resolve the user internally if needed, 
        // BUT wait, in our Register flow, the phone is in PendingUpdate, not User table.
        // CodeService ResolveUserIdAsync logic handles looking up PendingUpdates.

        // We construct the verify DTO
        var verifyDto = new Application.DTOs.Authentication.Verification.Requests.VerifyCodeDto
        {
            UserIdentifier = dto.PhoneNumber, // Or we could pass UserId if DTO allows
            Code = dto.Code,
            CodeType = CodeType.PhoneNumberConfirmation
        };

        var verifyResult = await _codeService.VerifyCodeAsync(verifyDto);
        if (!verifyResult.IsSuccess)
        {
            return Result<AuthResponse>.Failure(verifyResult.StatusCode);
        }

        // 2. Retrieve the Pending Update to get the confirmed phone number
        // We need the UserId first. Since CodeService verified it, we know the link exists.
        // We can look up the pending update by the phone number directly.
        var pendingUpdateResult = await _pendingUpdateService.GetPendingByInfoAsync(dto.PhoneNumber, UpdateType.NewRegistration);

        if (!pendingUpdateResult.IsSuccess || pendingUpdateResult.Value == null)
        {
            return Result<AuthResponse>.Failure(UserStatusCodes.UserNotFound);
        }

        var pendingUpdate = pendingUpdateResult.Value;
        var userId = pendingUpdate.UserId;

        // 3. Get the User Entity (Tracking enabled for update)
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return Result<AuthResponse>.Failure(UserStatusCodes.UserNotFound);

        // 4. Apply Updates (Activate & Set Phone)
        user.IsActive = true;
        user.PhoneNumber = pendingUpdate.UpdatedInfo; // Move phone from Pending to User
        user.PhoneNumberConfirmed = true;

        // 5. Create Session
        var sessionResult = await _sessionService.CreateSessionAsync(userId);
        if (!sessionResult.IsSuccess) return Result<AuthResponse>.Failure(sessionResult.StatusCode);
        var sessionId = sessionResult.Value;

        // 6. Generate Tokens
        // A. Grant Refresh Token (Update/Create in DB)
        var refreshTokenString = await _tokenService.GrantRefreshTokenAsync(user);
        // B. Generate Access Token
        var accessTokenString = _tokenService.GenerateAccessToken(user, sessionId);

        // 7. Archiving (Audit Trail)
        // A. Archive Identity (The new phone number)
        await _archiveService.ArchiveIdentityAsync(userId, user.PhoneNumber, IdentityType.PhoneNumber);
        // B. Archive Password (The initial password)
        await _archiveService.ArchivePasswordAsync(userId, user.PasswordHash);
        // C. Archive Action
        await _archiveService.LogUserActionAsync(userId, ActionType.AccountActivated, userId, "Account activated via SMS.");

        // 8. Cleanup
        // Remove the PendingUpdate record as it is fulfilled
        await _pendingUpdateService.DeleteAsync(pendingUpdate.Id);

        // 9. Commit Transaction (Updates User, adds Session, adds RefreshToken, adds Archives, removes Pending)
        await _unitOfWork.CompleteAsync();

        return Result<AuthResponse>.Success(
            new AuthResponse(accessTokenString, refreshTokenString),
            UserStatusCodes.UserAlreadyExists); // Should be "ActivationSuccess" status
    }

    // ========================================================================
    // Helpers
    // ========================================================================

    private async Task<string> GenerateUniqueUsernameAsync(string firstName, string lastName)
    {
        // Simple strategy: concat names + random number
        // In production, you might loop to check DB existence
        var baseName = $"{firstName.ToLower().Trim()}{lastName.ToLower().Trim()}";
        var randomSuffix = _generatorService.GenerateSecureCode().Substring(0, 4);
        var username = $"{baseName}{randomSuffix}";

        // Paranoid check (optional)
        while (await _userRepo.GetSingleByExpressionAsync(u => u.UserName == username) != null)
        {
            randomSuffix = _generatorService.GenerateSecureCode().Substring(0, 4);
            username = $"{baseName}{randomSuffix}";
        }

        return username;
    }

    // Placeholder for BCrypt or similar
    private string HashPassword(string password)
    {
        // Use BCrypt.Net.BCrypt.HashPassword(password);
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public Task<Result<Guid>> RegisterAsync(RegisterDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<AuthenticationResultDto>> ActiveAccountAsync(AccountActivationDto dto)
    {
        throw new NotImplementedException();
    }
}