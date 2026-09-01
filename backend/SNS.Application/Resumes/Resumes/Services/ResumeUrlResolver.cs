using Microsoft.EntityFrameworkCore;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Storage;

namespace SNS.Application.Resumes.Resumes.Services;

/// <summary>
/// Implements secure temporary URL resolution for resume pictures using <see cref="IFileStorageService"/>.
/// </summary>
public sealed class ResumeUrlResolver : IResumeUrlResolver
{
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public ResumeUrlResolver(
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }

    public async Task<string?> ResolvePersonalPictureUrlAsync(
        string? personalPictureKey,
        bool syncProfilePicture,
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        string? targetKey = null;

        if (syncProfilePicture)
        {
            targetKey = await _dbContext.Profiles
                .Where(p => p.Id == ownerId)
                .Select(p => p.ProfilePictureObjectKey)
                .FirstOrDefaultAsync(cancellationToken);
        }
        else
        {
            targetKey = personalPictureKey;
        }

        if (string.IsNullOrWhiteSpace(targetKey))
        {
            return null;
        }

        return await _fileStorageService.GetTemporaryUrlAsync(targetKey, DefaultExpiry);
    }

    public async Task<Dictionary<Guid, string?>> ResolvePersonalPictureUrlsBatchAsync(
        IEnumerable<ResumePictureResolutionRequest> requests,
        CancellationToken cancellationToken = default)
    {
        var requestList = requests.ToList();
        var result = new Dictionary<Guid, string?>();

        if (!requestList.Any())
        {
            return result;
        }

        var profileIdsToFetch = requestList
            .Where(r => r.SyncProfilePicture)
            .Select(r => r.OwnerId)
            .Distinct()
            .ToList();

        Dictionary<Guid, string?> profileKeyMap = new();
        if (profileIdsToFetch.Any())
        {
            profileKeyMap = await _dbContext.Profiles
                .Where(p => profileIdsToFetch.Contains(p.Id))
                .Select(p => new { p.Id, p.ProfilePictureObjectKey })
                .ToDictionaryAsync(p => p.Id, p => p.ProfilePictureObjectKey, cancellationToken);
        }

        // Map resume ID to target object key
        var resumeToKeyMap = new Dictionary<Guid, string?>();
        foreach (var req in requestList)
        {
            if (req.SyncProfilePicture)
            {
                profileKeyMap.TryGetValue(req.OwnerId, out var key);
                resumeToKeyMap[req.ResumeId] = key;
            }
            else
            {
                resumeToKeyMap[req.ResumeId] = req.PersonalPictureKey;
            }
        }

        var distinctKeys = resumeToKeyMap.Values
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct()
            .ToList();

        var keyUrlTasks = distinctKeys.Select(async k => new
        {
            Key = k!,
            Url = await _fileStorageService.GetTemporaryUrlAsync(k!, DefaultExpiry)
        });

        var resolvedKeyUrls = await Task.WhenAll(keyUrlTasks);
        var keyUrlMap = resolvedKeyUrls.ToDictionary(x => x.Key, x => x.Url);

        foreach (var req in requestList)
        {
            var key = resumeToKeyMap[req.ResumeId];
            if (!string.IsNullOrWhiteSpace(key) && keyUrlMap.TryGetValue(key, out var url))
            {
                result[req.ResumeId] = url;
            }
            else
            {
                result[req.ResumeId] = null;
            }
        }

        return result;
    }
}
