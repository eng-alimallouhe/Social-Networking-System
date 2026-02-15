using SNS.Application.DTOs.SocialGraph;
using SNS.Common.Results;

namespace SNS.Application.Abstractions.SocialGraph;

/// <summary>
/// Represents a domain service responsible for
/// managing user profiles within the social network.
/// 
/// This service encapsulates the business logic related to
/// profile creation, data updates, media management, and search capabilities,
/// while keeping the Application layer decoupled from infrastructure and implementation details.
/// </summary>
public interface IProfileService
{
    // ------------------------------------------------------------------
    // Command operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Creates a new profile for the authenticated user.
    /// 
    /// This operation is responsible for:
    /// - Validating that the user does not already have a profile.
    /// - Initializing the profile with the provided data.
    /// - Persisting the new entity to the social graph.
    /// </summary>
    /// <param name="dto">
    /// The data transfer object containing the initial profile details.
    /// </param>
    /// <returns>
    /// A <see cref="Result{Guid}"/> containing the unique identifier of the created profile
    /// if the operation completed successfully.
    /// </returns>
    Task<Result<Guid>> CreateProfileAsync(CreateProfileDto dto);

    /// <summary>
    /// Updates the basic information of a profile, such as full name and bio.
    /// 
    /// This operation is responsible for:
    /// - Validating the input data format.
    /// - Updating the core properties of the profile entity.
    /// </summary>
    /// <param name="profileId">
    /// The unique identifier of the profile to update.
    /// </param>
    /// <param name="dto">
    /// The data transfer object containing the updated basic information.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the operation
    /// completed successfully or failed with a business error.
    /// </returns>
    Task<Result> UpdateBasicInfoAsync(Guid profileId, UpdateBasicInfoDto dto);

    /// <summary>
    /// Updates the social and external links of a profile.
    /// 
    /// This operation is responsible for:
    /// - Validating the format of the provided URLs.
    /// - Persisting the collection of social links.
    /// </summary>
    /// <param name="profileId">
    /// The unique identifier of the profile to update.
    /// </param>
    /// <param name="dto">
    /// The data transfer object containing the updated links.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the operation
    /// completed successfully or failed with a business error.
    /// </returns>
    Task<Result> UpdateLinksAsync(Guid profileId, UpdateProfileLinksDto dto);

    /// <summary>
    /// Updates the geographical location of a profile.
    /// 
    /// This operation is responsible for:
    /// - normalizing location data.
    /// - Persisting the location details to the profile.
    /// </summary>
    /// <param name="profileId">
    /// The unique identifier of the profile to update.
    /// </param>
    /// <param name="dto">
    /// The data transfer object containing the updated location data.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the operation
    /// completed successfully or failed with a business error.
    /// </returns>
    Task<Result> UpdateLocationAsync(Guid profileId, UpdateProfileLocationDto dto);

    /// <summary>
    /// Updates the educational information of a profile.
    /// 
    /// This operation is responsible for:
    /// - Validating education entries.
    /// - Updating the education history associated with the profile.
    /// </summary>
    /// <param name="profileId">
    /// The unique identifier of the profile to update.
    /// </param>
    /// <param name="dto">
    /// The data transfer object containing the updated education history.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the operation
    /// completed successfully or failed with a business error.
    /// </returns>
    Task<Result> UpdateEducationAsync(Guid profileId, UpdateProfileEducationDto dto);

    /// <summary>
    /// Updates the profile image for the specified profile.
    /// 
    /// This operation is responsible for:
    /// - Validating the image file type and size.
    /// - Uploading the stream to the storage provider.
    /// - Updating the profile's avatar URL.
    /// </summary>
    /// <param name="profileId">
    /// The unique identifier of the profile.
    /// </param>
    /// <param name="imageStream">
    /// The stream containing the binary image data.
    /// </param>
    /// <param name="fileName">
    /// The original file name of the uploaded image.
    /// </param>
    /// <returns>
    /// A <see cref="Result{String}"/> containing the public URL of the uploaded image
    /// if the operation completed successfully.
    /// </returns>
    Task<Result<string>> UpdateProfileImageAsync(Guid profileId, Stream imageStream, string fileName);

    /// <summary>
    /// Updates the cover image for the specified profile.
    /// 
    /// This operation is responsible for:
    /// - Validating the image file type and size.
    /// - Uploading the stream to the storage provider.
    /// - Updating the profile's cover photo URL.
    /// </summary>
    /// <param name="profileId">
    /// The unique identifier of the profile.
    /// </param>
    /// <param name="imageStream">
    /// The stream containing the binary image data.
    /// </param>
    /// <param name="fileName">
    /// The original file name of the uploaded image.
    /// </param>
    /// <returns>
    /// A <see cref="Result{String}"/> containing the public URL of the uploaded image
    /// if the operation completed successfully.
    /// </returns>
    Task<Result<string>> UpdateCoverImageAsync(Guid profileId, Stream imageStream, string fileName);

    /// <summary>
    /// Permanently deletes the specified profile.
    /// 
    /// This operation is responsible for:
    /// - Removing the profile entity and associated data.
    /// - Cleaning up related resources (e.g., media files).
    /// </summary>
    /// <param name="profileId">
    /// The unique identifier of the profile to delete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the operation
    /// completed successfully or failed with a business error.
    /// </returns>
    Task<Result> DeleteProfileAsync(Guid profileId);

    // ------------------------------------------------------------------
    // Query operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Retrieves editable basic information for the specified profile.
    /// 
    /// This method does not mutate state and is intended for
    /// populating edit forms.
    /// </summary>
    /// <param name="profileId">
    /// The unique identifier of the profile.
    /// </param>
    /// <returns>
    /// A <see cref="Result{EditableBasicInfoDto}"/> containing the current basic information
    /// if found; otherwise, an appropriate failure result.
    /// </returns>
    Task<Result<EditableBasicInfoDto>> GetEditableBasicInfoAsync(Guid profileId);

    /// <summary>
    /// Retrieves editable profile links for the specified profile.
    /// 
    /// This method does not mutate state and is intended for
    /// populating edit forms.
    /// </summary>
    /// <param name="profileId">
    /// The unique identifier of the profile.
    /// </param>
    /// <returns>
    /// A <see cref="Result{EditableProfileLinksDto}"/> containing the current links configuration
    /// if found; otherwise, an appropriate failure result.
    /// </returns>
    Task<Result<EditableProfileLinksDto>> GetEditableLinksAsync(Guid profileId);

    /// <summary>
    /// Retrieves editable location information for the specified profile.
    /// 
    /// This method does not mutate state and is intended for
    /// populating edit forms.
    /// </summary>
    /// <param name="profileId">
    /// The unique identifier of the profile.
    /// </param>
    /// <returns>
    /// A <see cref="Result{EditableProfileLocationDto}"/> containing the current location data
    /// if found; otherwise, an appropriate failure result.
    /// </returns>
    Task<Result<EditableProfileLocationDto>> GetEditableLocationAsync(Guid profileId);

    /// <summary>
    /// Retrieves editable education information for the specified profile.
    /// 
    /// This method does not mutate state and is intended for
    /// populating edit forms.
    /// </summary>
    /// <param name="profileId">
    /// The unique identifier of the profile.
    /// </param>
    /// <returns>
    /// A <see cref="Result{EditableProfileEducationDto}"/> containing the current education history
    /// if found; otherwise, an appropriate failure result.
    /// </returns>
    Task<Result<EditableProfileEducationDto>> GetEditableEducationAsync(Guid profileId);

    /// <summary>
    /// Searches for profiles based on the provided filter criteria.
    /// 
    /// This method does not mutate state and is intended for
    /// discovery and lookup scenarios.
    /// </summary>
    /// <param name="filter">
    /// The criteria used to filter and retrieve profiles (e.g., name, skills, location).
    /// </param>
    /// <returns>
    /// A <see cref="Result{List}"/> containing a collection of matching profile summaries.
    /// </returns>
    Task<Result<List<ProfileSummaryDto>>> SearchProfilesAsync(ProfileSearchFilterDto filter);

    /// <summary>
    /// Retrieves detailed profile information for the specified target profile.
    /// 
    /// This method does not mutate state and is intended for
    /// viewing a full profile page.
    /// </summary>
    /// <param name="targetProfileId">
    /// The unique identifier of the profile being viewed.
    /// </param>
    /// <param name="viewerProfileId">
    /// The optional identifier of the viewing user, used to determine
    /// relationship context (e.g., "Following", "Friend").
    /// </param>
    /// <returns>
    /// A <see cref="Result{ProfileDetailsDto}"/> containing the full profile details
    /// if found; otherwise, an appropriate failure result.
    /// </returns>
    Task<Result<ProfileDetailsDto>> GetProfileDetailsAsync(Guid targetProfileId, Guid? viewerProfileId);
}