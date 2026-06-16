using SNS.Application.Abstractions.Caching;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Profiles.Profiles.Services;

public class ProfileCacheService : IProfileCacheService
{
    private readonly IProfileCacheKeyFactory _cacheKeyFactory;
    private readonly ICacheService _cacheService;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(20);

    public ProfileCacheService(
        IProfileCacheKeyFactory cacheKeyFactory,
        ICacheService cacheService,
        ISoftDeletableRepository<Profile> profileRepo)
    {
        _cacheKeyFactory = cacheKeyFactory;
        _cacheService = cacheService;
        _profileRepo = profileRepo;
    }

    public async Task<ProfileIntegrationModel?> GetProfileAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        var key = _cacheKeyFactory.GetProfileKey(profileId);

        var model = await _cacheService.GetAsync<ProfileIntegrationModel>(key, cancellationToken);

        if (model != null)
        {
            return model;
        }

        var fallBackProfile = await _profileRepo.GetByIdAsync(profileId, cancellationToken);

        if (fallBackProfile == null)
        {
            return null;
        }

        model = new ProfileIntegrationModel(profileId, fallBackProfile.FullName, fallBackProfile.UserId, fallBackProfile.ProfilePictureUrl, fallBackProfile.Specialization, fallBackProfile.IsActive);
        var mappingKey = _cacheKeyFactory.GetUserProfileMappingKey(fallBackProfile.UserId);

        await SetProfileAsync(model, cancellationToken);

        return model;
    }


    public async Task<ProfileIntegrationModel?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var mappingKey = _cacheKeyFactory.GetUserProfileMappingKey(userId);

        var profileId = await _cacheService.GetAsync<Guid?>(mappingKey, cancellationToken);

        if (!profileId.HasValue)
        {
            var fallBackProfile = await _profileRepo.GetSingleByExpressionAsync(
                p => p.UserId == userId, cancellationToken);

            if (fallBackProfile == null)
            {
                return null;
            }

            var model = new ProfileIntegrationModel(fallBackProfile.Id, fallBackProfile.FullName, fallBackProfile.UserId, fallBackProfile.ProfilePictureUrl, fallBackProfile.Specialization, fallBackProfile.IsActive);

            await SetProfileAsync(model, cancellationToken);

            return model;
        }
            
        return await GetProfileAsync(profileId.Value, cancellationToken);
    }

    public async Task<Result> RemoveProfileAsync(Guid profileId, Guid userId, CancellationToken cancellationToken = default)
    {
        var key = _cacheKeyFactory.GetProfileKey(profileId);

        var mappingKey = _cacheKeyFactory.GetUserProfileMappingKey(userId);  

        await _cacheService.RemoveAsync(key, cancellationToken);
        
        await _cacheService.RemoveAsync(mappingKey, cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }

    
    public async Task<Result> SetProfileAsync(ProfileIntegrationModel model, CancellationToken cancellationToken = default)
    {
        var key = _cacheKeyFactory.GetProfileKey(model.ProfileId);
        
        var mappingKey = _cacheKeyFactory.GetUserProfileMappingKey(model.UserId);

        await _cacheService.SetAsync(key, model, _cacheDuration, cancellationToken);
        await _cacheService.SetAsync(mappingKey, model.ProfileId, _cacheDuration, cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}
