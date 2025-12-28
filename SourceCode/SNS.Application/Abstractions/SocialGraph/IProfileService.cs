using SNS.Application.DTOs.SocialGraph;
using SNS.Common.Results;

namespace SNS.Application.Abstractions.SocialGraph
{
    /// <summary>
    /// Defines application-level operations for managing user profiles
    /// within the social network.
    ///
    /// This service acts as the main entry point for all profile-related
    /// use cases, including creation, updates, retrieval, search,
    /// media management, and deletion.
    ///
    /// The interface represents an application contract and contains
    /// no implementation or infrastructure-specific logic.
    /// </summary>
    public interface IProfileService
    {
        // ------------------------------------------------------------------
        // Profile Creation
        // ------------------------------------------------------------------

        /// <summary>
        /// Creates a new profile for the authenticated user.
        /// </summary>
        /// <param name="dto">
        /// The data required to create the profile.
        /// </param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the created profile identifier
        /// if the operation succeeds.
        /// </returns>
        Task<Result<Guid>> CreateProfileAsync(CreateProfileDto dto);

        // ------------------------------------------------------------------
        // Basic Information
        // ------------------------------------------------------------------

        /// <summary>
        /// Updates the basic information of a profile, such as full name and bio.
        /// </summary>
        /// <param name="profileId">
        /// The unique identifier of the profile to update.
        /// </param>
        /// <param name="dto">
        /// The updated basic information.
        /// </param>
        /// <returns>
        /// A <see cref="Result"/> indicating whether the update succeeded.
        /// </returns>
        Task<Result> UpdateBasicInfoAsync(Guid profileId, UpdateBasicInfoDto dto);

        /// <summary>
        /// Retrieves editable basic information for the specified profile.
        /// </summary>
        /// <param name="profileId">
        /// The unique identifier of the profile.
        /// </param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the editable basic information.
        /// </returns>
        Task<Result<EditableBasicInfoDto>> GetEditableBasicInfoAsync(Guid profileId);

        // ------------------------------------------------------------------
        // Profile Links
        // ------------------------------------------------------------------

        /// <summary>
        /// Updates the social and external links of a profile.
        /// </summary>
        /// <param name="profileId">
        /// The unique identifier of the profile.
        /// </param>
        /// <param name="dto">
        /// The updated profile links.
        /// </param>
        /// <returns>
        /// A <see cref="Result"/> indicating whether the update succeeded.
        /// </returns>
        Task<Result> UpdateLinksAsync(Guid profileId, UpdateProfileLinksDto dto);

        /// <summary>
        /// Retrieves editable profile links for the specified profile.
        /// </summary>
        /// <param name="profileId">
        /// The unique identifier of the profile.
        /// </param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the editable profile links.
        /// </returns>
        Task<Result<EditableProfileLinksDto>> GetEditableLinksAsync(Guid profileId);

        // ------------------------------------------------------------------
        // Location
        // ------------------------------------------------------------------

        /// <summary>
        /// Updates the geographical location of a profile.
        /// </summary>
        /// <param name="profileId">
        /// The unique identifier of the profile.
        /// </param>
        /// <param name="dto">
        /// The updated location data.
        /// </param>
        /// <returns>
        /// A <see cref="Result"/> indicating whether the update succeeded.
        /// </returns>
        Task<Result> UpdateLocationAsync(Guid profileId, UpdateProfileLocationDto dto);

        /// <summary>
        /// Retrieves editable location information for the specified profile.
        /// </summary>
        /// <param name="profileId">
        /// The unique identifier of the profile.
        /// </param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the editable location information.
        /// </returns>
        Task<Result<EditableProfileLocationDto>> GetEditableLocationAsync(Guid profileId);

        // ------------------------------------------------------------------
        // Education
        // ------------------------------------------------------------------

        /// <summary>
        /// Updates the educational information of a profile.
        /// </summary>
        /// <param name="profileId">
        /// The unique identifier of the profile.
        /// </param>
        /// <param name="dto">
        /// The updated education data.
        /// </param>
        /// <returns>
        /// A <see cref="Result"/> indicating whether the update succeeded.
        /// </returns>
        Task<Result> UpdateEducationAsync(Guid profileId, UpdateProfileEducationDto dto);

        /// <summary>
        /// Retrieves editable education information for the specified profile.
        /// </summary>
        /// <param name="profileId">
        /// The unique identifier of the profile.
        /// </param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the editable education information.
        /// </returns>
        Task<Result<EditableProfileEducationDto>> GetEditableEducationAsync(Guid profileId);

        // ------------------------------------------------------------------
        // Search
        // ------------------------------------------------------------------

        /// <summary>
        /// Searches for profiles based on the provided filter criteria.
        /// </summary>
        /// <param name="filter">
        /// The search filter containing criteria such as name, skills,
        /// interests, location, and education.
        /// </param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing a list of matching profile summaries.
        /// </returns>
        Task<Result<List<ProfileSummaryDto>>> SearchProfilesAsync(ProfileSearchFilterDto filter);

        // ------------------------------------------------------------------
        // Profile Details
        // ------------------------------------------------------------------

        /// <summary>
        /// Retrieves detailed profile information for the specified target profile.
        /// </summary>
        /// <param name="targetProfileId">
        /// The unique identifier of the profile being viewed.
        /// </param>
        /// <param name="viewerProfileId">
        /// The identifier of the viewing profile, if available,
        /// used to determine relationship-specific data.
        /// </param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the full profile details.
        /// </returns>
        Task<Result<ProfileDetailsDto>> GetProfileDetailsAsync(
            Guid targetProfileId,
            Guid? viewerProfileId);

        // ------------------------------------------------------------------
        // Profile Media
        // ------------------------------------------------------------------

        /// <summary>
        /// Updates the profile image for the specified profile.
        /// </summary>
        /// <param name="profileId">
        /// The unique identifier of the profile.
        /// </param>
        /// <param name="imageStream">
        /// The stream containing the profile image data.
        /// </param>
        /// <param name="fileName">
        /// The original file name of the image.
        /// </param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the URL of the uploaded image.
        /// </returns>
        Task<Result<string>> UpdateProfileImageAsync(
            Guid profileId,
            Stream imageStream,
            string fileName);

        /// <summary>
        /// Updates the cover image for the specified profile.
        /// </summary>
        /// <param name="profileId">
        /// The unique identifier of the profile.
        /// </param>
        /// <param name="imageStream">
        /// The stream containing the cover image data.
        /// </param>
        /// <param name="fileName">
        /// The original file name of the image.
        /// </param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the URL of the uploaded image.
        /// </returns>
        Task<Result<string>> UpdateCoverImageAsync(
            Guid profileId,
            Stream imageStream,
            string fileName);

        // ------------------------------------------------------------------
        // Deletion
        // ------------------------------------------------------------------

        /// <summary>
        /// Permanently deletes the specified profile.
        /// </summary>
        /// <param name="profileId">
        /// The unique identifier of the profile to delete.
        /// </param>
        /// <returns>
        /// A <see cref="Result"/> indicating whether the deletion succeeded.
        /// </returns>
        Task<Result> DeleteProfileAsync(Guid profileId);
    }
}
