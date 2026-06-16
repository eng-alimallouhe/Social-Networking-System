namespace SNS.Application.Profiles.Profiles.abstractions;

public interface IProfileCacheKeyFactory
{
    string GetUserProfileMappingKey(Guid userId);
    string GetProfileKey(Guid profileId);
}
