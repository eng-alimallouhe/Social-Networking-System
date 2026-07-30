using SNS.Application.Abstractions.Loggings;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Profiles.Profiles.Commands.UpdateProfilePicture;

/// <summary>
/// Handles the execution of <see cref="UpdateProfilePictureCommand"/> to update a profile picture.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID and fetches the profile entity.
/// 2. Uploads the new profile picture to file storage.
/// 3. Updates the profile picture object key on the entity and saves database changes.
/// 4. Deletes the previous profile picture file from storage if one existed.
/// 5. Removes the uploaded file if database persistence fails.
/// Side effects include storage file upload and deletion, profile entity property modification, and database transaction commit.
/// </remarks>
internal sealed class UpdateProfilePictureCommandHandler : ICommandHandler<UpdateProfilePictureCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAppLogger<UpdateProfilePictureCommandHandler> _logger;

    public UpdateProfilePictureCommandHandler(
        ICurrentUserService currentUserService,
        ISoftDeletableRepository<Profile> profileRepo,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IAppLogger<UpdateProfilePictureCommandHandler> logger)
    {
        _currentUserService = currentUserService;
        _profileRepo = profileRepo;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;

        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var profile = await  _profileRepo.GetByIdAsync(profileId.Value, cancellationToken);

        if (profile == null)
        {
            return Result.Failure(UserStatusCodes.ProfileNotCompleted);
        }


        bool fileUploaded = false;

        string? newProfilePictureObjectKey = null;

        var oldProfilePictureObjectKey = profile.ProfilePictureObjectKey;
        try
        {
            newProfilePictureObjectKey = $"profiles/{profileId.Value}/avatar_avatar_{Guid.NewGuid():N}.{request.ProfilePictureFile.Extension}";

            await _fileStorageService.UploadFileAsync(
                fileStream: request.ProfilePictureFile.Stream, 
                contentType: request.ProfilePictureFile.ContentType,
                objectKey: newProfilePictureObjectKey,
                cancellationToken: cancellationToken);

            fileUploaded = true;


            profile.UpdateProfilePictureObjectKey(newProfilePictureObjectKey);

            await _unitOfWork.CompleteAsync(cancellationToken);

            if (oldProfilePictureObjectKey != null)
            {
                try
                {
                    await _fileStorageService.DeleteFileAsync(oldProfilePictureObjectKey, cancellationToken);
                }
                catch (Exception ex) 
                { 
                    _logger.LogError("Failed to delete old profile picture with object key: {OldProfilePictureObjectKey}", ex, oldProfilePictureObjectKey);
                }
            }

            return Result.Success(OperationStatusCode.Success);
        }
        catch
        {
            if (fileUploaded)
            {
                await _fileStorageService.DeleteFileAsync(newProfilePictureObjectKey!, cancellationToken);
            }
            throw;
        }
    }
}