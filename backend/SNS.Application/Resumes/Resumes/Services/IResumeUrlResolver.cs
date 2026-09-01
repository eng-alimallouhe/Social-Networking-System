namespace SNS.Application.Resumes.Resumes.Services;

/// <summary>
/// Information required to resolve a resume's display picture URL.
/// </summary>
/// <param name="ResumeId">The unique identifier of the resume.</param>
/// <param name="OwnerId">The profile ID of the resume owner.</param>
/// <param name="PersonalPictureKey">The storage object key stored on the resume (if any).</param>
/// <param name="SyncProfilePicture">Whether the resume uses the profile avatar.</param>
public sealed record ResumePictureResolutionRequest(
    Guid ResumeId,
    Guid OwnerId,
    string? PersonalPictureKey,
    bool SyncProfilePicture
);

/// <summary>
/// Service interface for securely resolving presigned temporary URLs for resume personal and profile pictures.
/// </summary>
public interface IResumeUrlResolver
{
    /// <summary>
    /// Resolves a single temporary picture URL based on sync settings and storage keys.
    /// </summary>
    Task<string?> ResolvePersonalPictureUrlAsync(
        string? personalPictureKey,
        bool syncProfilePicture,
        Guid ownerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves temporary picture URLs for a batch of resumes efficiently without N+1 overhead.
    /// </summary>
    Task<Dictionary<Guid, string?>> ResolvePersonalPictureUrlsBatchAsync(
        IEnumerable<ResumePictureResolutionRequest> requests,
        CancellationToken cancellationToken = default);
}
