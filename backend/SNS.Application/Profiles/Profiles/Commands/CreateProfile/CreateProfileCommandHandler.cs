using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.Users;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Profiles.Profiles.Commands.CreateProfile;

/// <summary>
/// Handles the execution of <see cref="CreateProfileCommand"/> to create a user profile.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Verifies authentication and active status of the requesting user.
/// 2. Ensures the user does not already possess a profile.
/// 3. Uploads profile picture file to storage if provided.
/// 4. Creates and persists the <see cref="Profile"/> entity within a database transaction.
/// 5. Cleans up uploaded storage file if database commit fails.
/// Side effects include avatar storage upload, profile entity creation, and transaction persistence.
/// </remarks>
internal sealed class CreateProfileCommandHandler : ICommandHandler<CreateProfileCommand, AuthTokensDto>
{
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IProfileCacheService _profileCacheService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUserCacheService _userCacheService;
    private readonly IAuthenticationFlowService _authenticationFlowService;

    public CreateProfileCommandHandler(
        ISoftDeletableRepository<Profile> profileRepo,
        IProfileCacheService profileCacheService,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        IUserCacheService userCacheService,
        IAuthenticationFlowService authenticationFlowService)
    {
        _profileRepo = profileRepo;
        _profileCacheService = profileCacheService;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
        _userCacheService = userCacheService;
        _authenticationFlowService = authenticationFlowService;
    }

    public async Task<Result<AuthTokensDto>> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var sessionId = _currentUserService.SessionId;

        string? profilePictureObjectKey = null;

        var fileUploaded = false;

        if (userId == null || sessionId == null)
        {
            return Result<AuthTokensDto>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var user = await _userCacheService.GetUserAsync(userId.Value, cancellationToken);

        if (user == null || user.Status != UserStatus.Active)
        {
            return Result<AuthTokensDto>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }


        var oldProfile = await _profileCacheService.GetProfileByUserIdAsync(userId.Value);

        if (oldProfile != null)
        {
            return Result<AuthTokensDto>.Failure(OperationStatusCode.Conflict);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var profile = Profile.Create(
                userId: userId.Value,
                fullName: request.FullName, 
                bio: request.Bio,
                specialization: request.Specialization, 
                profilePictureObjectKey: null);

            if (request.ProfilePicture != null)
            {
                var objectKey = $"profiles/{profile.Id}/avatar_{Guid.NewGuid():N}.{request.ProfilePicture.Extension}";
                
                await _fileStorageService.UploadFileAsync(
                    fileStream: request.ProfilePicture.Stream,
                    contentType: request.ProfilePicture.ContentType,
                    objectKey: objectKey,
                    cancellationToken: cancellationToken);

                fileUploaded = true;
                
                profilePictureObjectKey = objectKey;

                profile.UpdateProfilePictureObjectKey(objectKey);
            }

            _profileRepo.Add(profile);

            var authenticateResult = await _authenticationFlowService.AuthenticateUserAsync(
                new AuthenticateUserRequest(
                    UserId: user.UserId,
                    RoleId: user.RoleId,
                    RoleType: user.RoleType,
                    ProfileId: profile.Id,
                    SessionId: sessionId));

            if (authenticateResult.IsFailure || authenticateResult.Value == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return authenticateResult;
            }

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            return Result<AuthTokensDto>.Success(new AuthTokensDto(
                Token: authenticateResult.Value.Token,
                RefreshToken: authenticateResult.Value.RefreshToken), OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            if (fileUploaded)
            {
                await _fileStorageService.DeleteFileAsync(profilePictureObjectKey!, cancellationToken);
            }
            throw;
        }
    }
}